using UnityEngine;

namespace MapGeneration.Rooms
{
	public class PrisonRoom : RoomTrigger
	{
		[SerializeField] private PrisonSpawner _prisonSpawner;
		private void Start()
		{
			OnPlayerEnter += OnPlayerEnterHandler;
			_prisonSpawner = Instantiate(_prisonSpawner, MeshBulilder.I.ConvertPoint(Room.GetRandomPoint()),
				Quaternion.identity, transform);
		}

		private void OnPlayerEnterHandler()
		{
			if (_prisonSpawner.IsSpawned) return;
			_prisonSpawner.StartSpawn();
		}
	}
}