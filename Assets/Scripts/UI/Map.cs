using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.MapGeneration;
using MapGeneration.BSP;
using MapGeneration.Rooms;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class Map : MonoBehaviour
{
	[SerializeField] private bool _showFullMap = false;

	public static Map Instance;

	private RawImage _image;
	private Texture2D _texture;

	private bool _isOpen;

	private void Awake()
	{
		if (Instance != null)
			Destroy(gameObject);
		else
			Instance = this;
		_image = GetComponent<RawImage>();

		//MeshBulilder.I.OnMapGenerated += OnMapGenerated;
	}

	private void Start()
	{
		MeshBulilder.I.OnMapGenerated += OnMapGenerated;
		if (MeshBulilder.I.Generated)
			OnMapGenerated(MeshBulilder.I.TilemapGenerator);
		gameObject.SetActive(false);
	}

	private void OnMapGenerated(TilemapGenerator generator)
	{
		var roomTriggers = MeshBulilder.I.GetRoomTriggers();
		_texture = new Texture2D(MeshBulilder.I.MapSize.x, MeshBulilder.I.MapSize.y);
		for (int x = 0; x < MeshBulilder.I.MapSize.x; x++)
		{
			for (int y = 0; y < MeshBulilder.I.MapSize.y; y++)
			{
				_texture.SetPixel(x, y, Color.black);
			}
		}

		_texture.Apply();
		_image.texture = _texture;
		foreach (var roomTrigger in roomTriggers)
		{
			roomTrigger.OnPlayerEnter += () => OnPlayerEnter(roomTrigger);
		}

		if (_showFullMap)
		{
			foreach (var roomTrigger in roomTriggers)
			{
				Show(roomTrigger.Room, false);
			}
		}
	}

	private void OnPlayerEnter(RoomTrigger trigger)
	{
		Show(trigger.Room, false);
	}

	private void Show(Room triggerRoom, bool isCorridor)
	{
		Color color = Color.white;
		if (!isCorridor)
		{
			color = GetRoomColor(triggerRoom.Type);
		}
		else
		{
			color = new Color(235, 137, 76);
		}

		for (int x = triggerRoom.GetLeft(); x <= triggerRoom.GetRight(); x++)
		{
			for (int y = triggerRoom.GetBottom(); y <= triggerRoom.GetTop(); y++)
			{
				_texture.SetPixel(x, y, color);
			}
		}

		var corridors = triggerRoom.GetCorridors();
		if (!isCorridor)
		{
			foreach (var corridor in corridors)
			{
				Show(corridor, true);
			}
		}

		_texture.Apply();
	}

	private Color GetRoomColor(RoomType type)
	{
		switch (type)
		{
			case RoomType.Battle:
				return Color.red;

			case RoomType.Spawn:
				return Color.green;

			case RoomType.Prison:
				return Color.grey;

			case RoomType.Fountain:
				return Color.blue;

			case RoomType.Exit:
				return Color.yellow;

			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}

	public void ToggleMap()
	{
		if (_isOpen)
		{
			gameObject.SetActive(false);
			_isOpen = false;
		}
		else
		{
			gameObject.SetActive(true);
			_isOpen = true;
		}
	}
}