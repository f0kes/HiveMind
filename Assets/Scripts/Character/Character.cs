using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using DefaultNamespace;
using MapGeneration.BSP;
using MapGeneration.Rooms;
using UnityEngine;

[RequireComponent(typeof(CharacterShooter))]
[RequireComponent(typeof(CharacterInteractor))]
public class Character : Entity
{
	public float AIDesirability = 1;
	public float AIThreat = 1;

	private bool _swapped;
	public bool Swapped => _swapped;
	private Character _swapTarget;

	public CharacterControlsProvider ControlsProvider;
	public CharacterMover CharacterMover { get; private set; }
	public CharacterShooter CharacterShooter { get; private set; }
	public CharacterInteractor CharacterInteractor { get; private set; }

	protected override void ChildAwake()
	{
		gameObject.layer = LayerMask.NameToLayer("Character");
		CharacterMover = gameObject.GetComponent<CharacterMover>();
		if (CharacterMover == null)
		{
			CharacterMover = gameObject.AddComponent<CharacterMover>();
		}

		CharacterShooter = gameObject.GetComponent<CharacterShooter>();
		CharacterInteractor = gameObject.GetComponent<CharacterInteractor>();


		CharacterInteractor.Init(this);
		CharacterShooter.Init(this);
	}

	public void SwapWithNew(Character other)
	{
		_swapped = true;
		_swapTarget = other;

		Swap(other);
	}

	public void SwapBack()
	{
		_swapped = false;
		Swap(_swapTarget);
		_swapTarget = null;
	}

	private void Swap(Character other)
	{
		other.ControlsProvider.SetCharacter(this);
		ControlsProvider.SetCharacter(other);

		(other.ControlsProvider, ControlsProvider) = (ControlsProvider, other.ControlsProvider);
	}

	public void TeleportTeam(RoomTrigger roomTrigger, float radius = 1.8f, float rayHeight = 10f)
	{
		List<Entity> team = EntityList.GetEntitiesOnTeam(Team);
		List<Character> characters = team.OfType<Character>().ToList();
		foreach (var teammate in characters)
		{
			teammate.Teleport(transform.position, roomTrigger, radius, rayHeight);
		}
	}

	public void Teleport(Vector3 position, RoomTrigger roomTrigger, float radius = 1.8f, float rayHeight = 10f)
	{
		var foundPoint = false;
		var iterations = 0;
		// Room room = MeshBulilder.I.GetRoom(position);
		// if(room == null)
		// {
		// 	Debug.DrawLine(position, position + Vector3.up * rayHeight, Color.red, 10000f);
		// }

		while (!foundPoint && iterations < 3500)
		{
			var offset = new Vector3(UnityEngine.Random.Range(-radius, radius), 0,
				UnityEngine.Random.Range(-radius, radius));
			var point = position + offset;
			var rayOrigin = new Vector3(point.x, rayHeight, point.z);
			bool isInRoom = roomTrigger.Collider.bounds.Contains(point);
			if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, 1000f, ~LayerMask.GetMask("RoomTrigger")))
			{
				Debug.DrawLine(rayOrigin, hit.point, Color.yellow, 1010f);
				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") &&
				    isInRoom) // there was a room check
				{
					foundPoint = true;
					transform.position = point;
				}
			}

			iterations++;
		}

		if (!foundPoint)
		{
			Debug.LogError("Could not find a place to teleport");
		}
	}
}