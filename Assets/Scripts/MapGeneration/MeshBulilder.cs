using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.MapGeneration;
using MapGeneration;
using MapGeneration.BSP;
using MapGeneration.Rooms;
using MapGeneration.Voxel;
using Player;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshBulilder : MonoBehaviour
{
	public event Action<TilemapGenerator> OnMapGenerated;

	public static MeshBulilder I;

	[SerializeField] private Vector3 _tileSize = new Vector3(1, 2, 1);
	[SerializeField] private Vector3 _defaultOffset = new Vector3(0, 0, 0);
	private Vector3 _offset = new Vector3(0, 0, 0);
	[SerializeField] private Vector2 _mapSize = new Vector2(100, 100);
	[SerializeField] private int _chunkSize = 10;
	[SerializeField] private VoxelRenderer _voxelRenderer;
	[SerializeField] private int _generationTries = 5;

	[SerializeField] private BattleRoom _battleRoom;
	[SerializeField] private SpawnRoom _spawnRoom;
	[SerializeField] private PrisonRoom _prisonRoom;
	[SerializeField] private FountainRoom _fountainRoom;
	[SerializeField] private ExitRoom _exitRoom;


	[SerializeField] private float _colorSatuation = 0.5f;
	[SerializeField] private float _wallColorDifference = 0.5f;
	[SerializeField] private int _level = 0;
	[SerializeField] private MeshRenderer _floorRenderer;

	private List<RoomTrigger> _roomTriggers = new List<RoomTrigger>();

	private List<VoxelRenderer> _voxelRenderers = new List<VoxelRenderer>();
	private List<VoxelRenderer> _pathRenderers = new List<VoxelRenderer>();
	public TilemapGenerator TilemapGenerator { get; private set; }
	private TileMap _currentTileMap;

	
	private RoomTrigger _currentSpawnRoom;
	private Vector2Int _playerSpawn;

	private static readonly int ColorProperty = Shader.PropertyToID("_Color");


	public bool Generated { get; private set; } = false;

	public Vector2Int MapSize => new Vector2Int((int) _mapSize.x, (int) _mapSize.y);

	// Start is called before the first frame update

	private void Awake()
	{
		if (I == null)
			I = this;
		else
			Destroy(gameObject);
	}

	void Start()
	{
		Generate();
	}

	private void Generate(int triesSoFar = 0)
	{
		if (triesSoFar > _generationTries)
		{
			Debug.LogError("Generation failed");
			return;
		}

		TextMessageRenderer.Instance.ShowBlackScreen("LEVEL: " + _level, 1.5f);


		TilemapGenerator = new TilemapGenerator((int) _mapSize.x, (int) _mapSize.y);
		_offset = new Vector3(_defaultOffset.x - (_mapSize.x / 2 * _tileSize.x), _defaultOffset.y,
			_defaultOffset.z - (_mapSize.y / 2 * _tileSize.z));
		var tilemap = TilemapGenerator.GenerateTilemap();
		_currentTileMap = tilemap;
		var rooms = TilemapGenerator.Rooms;

		if (!MarkRooms(rooms))
		{
			Generate(triesSoFar + 1);
			return;
		}
		MakeRooms(rooms);

		BuildMap(tilemap, _voxelRenderers);
		BuildMap(GetPaths(), _pathRenderers);

		
		transform.rotation = Quaternion.Euler(0, 45, 0);
		OnMapGenerated?.Invoke(TilemapGenerator);
		Generated = true;


		StartCoroutine(TeleportPlayerToSpawn());
	}

	private (Color floorColor, Color wallColor) GetRandomColors(int seed = -1)
	{
		if (seed == -1)
			seed = _level;

		Random.InitState(seed);
		var floorColor = new Color
		(UnityEngine.Random.Range(0f, _colorSatuation),
			UnityEngine.Random.Range(0f, _colorSatuation),
			UnityEngine.Random.Range(0f, _colorSatuation));
		var wallColor = new Color
		(floorColor.r + _wallColorDifference,
			floorColor.g + _wallColorDifference,
			floorColor.b + _wallColorDifference);

		return (floorColor, wallColor);
	}

	private void SetFloorColor(Color color)
	{
		Material newMaterial = new Material(_floorRenderer.material);
		newMaterial.SetColor(ColorProperty, color);
		_floorRenderer.material = newMaterial;
	}

	private IEnumerator TeleportPlayerToSpawn()
	{
		yield return new WaitForSeconds(0.15f);
		Vector3 playerSpawnConverted = ConvertPoint(_playerSpawn);
		Character.Character player = InputHandler.Instance.GetControlledCharacter();
		player.Teleport(playerSpawnConverted, _currentSpawnRoom, 3f);
		player.TeleportTeam(_currentSpawnRoom,3f);
		UnblockPaths();
	}

	private bool MarkRooms(List<Room> rooms)
	{
		if (rooms.Count < 5) return false;
			foreach (var room in rooms)
		{
			room.Type = RoomType.Battle;
		}

		var unmarkedRooms = new List<Room>(rooms);

		foreach (var type in (RoomType[]) Enum.GetValues(typeof(RoomType)))
		{
			if (type == RoomType.Battle)
				continue;

			var room = GetRandomRoom(unmarkedRooms);
			if (room == null)
				return false;
			room.Type = type;
			unmarkedRooms.Remove(room);
		}

		return true;
	}

	private static Room GetRandomRoom(IReadOnlyList<Room> unmarkedRooms)
	{
		if (unmarkedRooms.Count == 0)
			return null;
		var index = UnityEngine.Random.Range(0, unmarkedRooms.Count);
		return unmarkedRooms[index];
	}

	private void MakeRooms(List<Room> rooms)
	{
		foreach (var room in _roomTriggers)
		{
			Destroy(room.gameObject);
		}

		_roomTriggers.Clear();
		foreach (var room in rooms)
		{
			switch (room.Type)
			{
				case RoomType.Battle:
					MakeBattleRoom(room);
					break;

				case RoomType.Spawn:
					MakeSpawnRoom(room);
					break;
				case RoomType.Prison:
					MakePrisonRoom(room);
					break;
				case RoomType.Fountain:
					MakeFountainRoom(room);
					break;
				case RoomType.Exit:
					MakeExitRoom(room);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void MakeExitRoom(Room room)
	{
		var trigger = (ExitRoom) MakeGenericRoom(_exitRoom, room);
	}

	private void MakeFountainRoom(Room room)
	{
		var trigger = (FountainRoom) MakeGenericRoom(_fountainRoom, room);
	}

	private void MakePrisonRoom(Room room)
	{
		var trigger = (PrisonRoom) MakeGenericRoom(_prisonRoom, room);
		trigger.gameObject.name = "PrisonRoom";
	}

	private void MakeSpawnRoom(Room room)
	{
		var trigger = (SpawnRoom) MakeGenericRoom(_spawnRoom, room);
		_currentSpawnRoom = trigger;
		_playerSpawn = room.GetRandomPoint();
	}

	private void MakeBattleRoom(Room room)
	{
		var trigger = (BattleRoom) MakeGenericRoom(_battleRoom, room);

		trigger.OnBattleStart += BlockPaths;
		trigger.OnBattleEnd += UnblockPaths;
		trigger.SetSpawnerCount(_level);
	}

	private RoomTrigger MakeGenericRoom(RoomTrigger roomTrigger, Room room)
	{
		Transform transform1;
		var trigger = Instantiate(roomTrigger, ConvertPoint(room.GetCenter()), (transform1 = transform).rotation,
			transform1);
		trigger.SetRoom(room);
		trigger.transform.localScale =
			new Vector3(room.GetWidth() * _tileSize.x, _tileSize.y, room.GetHeight() * _tileSize.z);
		_roomTriggers.Add(trigger);
		return trigger;
	}

	private TileMap GetPaths()
	{
		TileMap tileMap = new TileMap(_currentTileMap);
		for (int x = 0; x < tileMap.XSize; x++)
		{
			for (int z = 0; z < tileMap.ZSize; z++)
			{
				if (tileMap[x, z] == (int) TileType.Wall)
				{
					tileMap[x, z] = (int) TileType.Empty;
				}

				if (tileMap[x, z] == (int) TileType.Corridor)
				{
					tileMap[x, z] = (int) TileType.Wall;
				}
			}
		}

		return tileMap;
	}

	private void UnblockPaths()
	{
		Debug.Log("unblock");
		foreach (var pathRenderer in _pathRenderers)
		{
			pathRenderer.gameObject.SetActive(false);
		}
	}

	private void BlockPaths()
	{
		foreach (var pathRenderer in _pathRenderers)
		{
			pathRenderer.gameObject.SetActive(true);
		}
	}

	public void NextLevel()
	{
		_level++;
		Generate();
	}

	public Vector3 ConvertPoint(Vector2Int point)
	{
		Vector3 result = new Vector3(point.x * _tileSize.x + _offset.x, _offset.y, point.y * _tileSize.z + _offset.z);
		result = transform.TransformPoint(result);
		return result;
	}

	public Vector2Int ConvertPoint(Vector3 point)
	{
		Vector3 result = transform.InverseTransformPoint(point);
		//result -= _offset;
		result.x = Mathf.FloorToInt((result.x - _offset.x) / _tileSize.x);
		result.z = Mathf.FloorToInt((result.z - _offset.z) / _tileSize.z);
		result.y = 0;
		Vector2Int result2 = new Vector2Int((int) result.x, (int) result.z);

		return (result2);
	}

	public List<RoomTrigger> GetRoomTriggers()
	{
		return new List<RoomTrigger>(_roomTriggers);
	}

	private void BuildMap(TileMap tilemap, List<VoxelRenderer> voxelRenderers)
	{
		foreach (var voxelRenderer in voxelRenderers)
		{
			if (voxelRenderer != null)
				Destroy(voxelRenderer.gameObject);
		}

		voxelRenderers.Clear();

		var chunkQuantityX = tilemap.XSize / _chunkSize;
		var chunkQuantityZ = tilemap.ZSize / _chunkSize;
		for (var x = 0; x < chunkQuantityX; x++)
		{
			for (var z = 0; z < chunkQuantityZ; z++)
			{
				var xMin = x * _chunkSize;
				var xMax = xMin + _chunkSize;
				var zMin = z * _chunkSize;
				var zMax = zMin + _chunkSize;

				var position = new Vector3(x * _chunkSize * _tileSize.x, 0, z * _chunkSize * _tileSize.z) + _offset;

				var chunk = tilemap.GetSlice(xMin, xMax, zMin, zMax);
				var chunkRenderer = Instantiate(_voxelRenderer, transform);
				voxelRenderers.Add(chunkRenderer);
				chunkRenderer.RenderVoxels(chunk, _tileSize, position);
			}
		}

		if (_level != 0)
		{
			var (floorColor, wallColor) = GetRandomColors();
			SetFloorColor(floorColor);
			foreach (var voxelRenderer in voxelRenderers)
			{
				voxelRenderer.SetColor(wallColor);
			}
		}
	}

	public Room GetRoom(Vector3 position)
	{
		//List<Room> rooms = TilemapGenerator.Rooms;
		return TilemapGenerator.Rooms.FirstOrDefault(room => room.Contains(ConvertPoint(position)));
		//find closest room
		/*Room closestRoom = null;
		float closestDistance = float.MaxValue;
		foreach (var room in rooms)
		{
			var distance = Vector3.Distance(ConvertPoint(room.GetCenter()), position);
			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestRoom = room;
			}
		}

		return closestRoom;*/
	}

	public int GetLevel()
	{
		return _level;
	}
}