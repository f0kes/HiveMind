using System.Collections.Generic;
using MapGeneration.Rooms;
using UI;
using UnityEngine;

namespace DefaultNamespace.Content
{
	[RequireComponent(typeof(Interactable))]
	public class Fountain : MonoBehaviour
	{
		[SerializeField] private MeshRenderer _waterRenderer;
		private Interactable _interactable;
		private RoomTrigger _room;

		private void Awake()
		{
			_interactable = GetComponent<Interactable>();
			_interactable.OnInteractWho += Interact;
		}
		public void SetRoom(RoomTrigger room)
		{
			_room = room;
		}

		private void Interact(Character character)
		{
			var interactorTeammates = EntityList.GetEntitiesOnTeam(character.Team);
			var interactorGraveyard = EntityList.GetGraveyard(character.Team);
			_waterRenderer.enabled = false;
			foreach (var entity in interactorTeammates)
			{
				entity.TakeFullRestore();
			}

			foreach (var entity in interactorGraveyard)
			{
				entity.Ressurect();
			}

			character.TeleportTeam(_room);
			TextMessageRenderer.Instance.ShowMessage("ALL CHARACTERS RESTORED", 3f);
		}
	}
}