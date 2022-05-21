using System;
using DefaultNamespace.Content;
using Unity.Mathematics;
using UnityEngine;

namespace MapGeneration.Rooms
{
	public class FountainRoom : RoomTrigger
	{
		[SerializeField] private Fountain _fountainPrefab;

		private void Start()
		{
			Fountain fountain = Instantiate(_fountainPrefab, MeshBulilder.I.ConvertPoint(Room.GetCenter()), quaternion.identity);
			fountain.transform.SetParent(transform,true);
			fountain.SetRoom(this);
		}
	}
}