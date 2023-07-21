using System;
using MapGeneration.BSP;
using Player;
using UnityEngine;

namespace MapGeneration.Rooms
{
	public class RoomTrigger : MonoBehaviour
	{
		public Room Room;
		public event Action OnPlayerEnter;
		public event Action OnPlayerExit;
		public bool IsActive;
		
		public Collider Collider{get; private set; }
		


		private void Awake()
		{
			Collider = GetComponent<Collider>();
		}

		public void SetRoom(Room room)
		{
			Room = room;	
		}
		private void OnTriggerEnter(Collider other)
		{
			var character = other.GetComponent<Characters.Character>();
			if (character!= null && character==InputHandler.Instance.GetControlledCharacter())
			{
				OnPlayerEnter?.Invoke();
				character.TeleportTeam(this);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			var character = other.GetComponent<Characters.Character>();
			if (character!= null && character==InputHandler.Instance.GetControlledCharacter())
			{
				OnPlayerExit?.Invoke();
			}
		}
	}
}