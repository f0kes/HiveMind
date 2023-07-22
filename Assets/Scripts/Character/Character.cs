using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Combat;
using Combat.Spells;
using Combat.Spells.AutoAttack;
using Combat.Spells.Heal;
using DefaultNamespace;
using DefaultNamespace.Settings;
using Enums;
using GameState;
using MapGeneration.Rooms;
using Stats;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Characters
{
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
		public CharacterMover CharacterMover{get; private set;}
		public CharacterShooter CharacterShooter{get; private set;}
		public CharacterInteractor CharacterInteractor{get; private set;}

		[SerializeField] private List<BaseSpell> _spells = new();

		public BaseSpell Spell => _spells.Count != 0 ? _spells[0] : null;


		protected override void ChildAwake()
		{
			
			gameObject.layer = LayerMask.NameToLayer("Character");
			CharacterMover = gameObject.GetComponent<CharacterMover>();
			if(CharacterMover == null)
			{
				CharacterMover = gameObject.AddComponent<CharacterMover>();
			}

			CharacterShooter = gameObject.GetComponent<CharacterShooter>();
			CharacterInteractor = gameObject.GetComponent<CharacterInteractor>();


			CharacterInteractor.Init(this);
			CharacterShooter.Init(this);

			
		}
		public void InitSpell(BaseSpell spell)
		{
			spell = Instantiate(spell);
			spell.SetOwner(this);
			spell.OnCreated();
			_spells.Add(spell);
		}
		public void InitSpells()
		{
			if(_spells.Count == 0)
			{
				var heal = BasicHealSpell.CreateDefault();
				var attack = BaseSpell.CreateDefault();
				InitSpell(heal);
				InitSpell(attack);
			}
			foreach(var copy in new List<BaseSpell>(_spells.Select(Instantiate)))
			{
				InitSpell(copy);
			}
		}


		protected override void ChildStart()
		{
			base.ChildStart();
			InitSpells();
			SubscribeToEvents();
		}
		private void OnDestroy()
		{
			UnSubscribeFromEvents();
		}
		private void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
		}
		private void UnSubscribeFromEvents()
		{
			Ticker.OnTick -= OnTick;
		}
		private void OnTick(Ticker.OnTickEventArgs obj)
		{
			
		}

		public void CastSpell(int index = 0)
		{
			if(index < _spells.Count)
			{
				_spells[index].Cast();
			}
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
			List<Entity> team = GlobalEntities.GetEntitiesOnTeam(Team);
			List<Character> characters = team.OfType<Character>().ToList();
			foreach(var teammate in characters)
			{
				teammate.Teleport(transform.position, roomTrigger, radius, rayHeight);
			}
		}

		public void Teleport(Vector3 position, RoomTrigger roomTrigger, float radius = 1.8f, float rayHeight = 10f)
		{
			var foundPoint = false;
			var iterations = 0;
			while (!foundPoint && iterations < 3500)
			{
				var offset = new Vector3(Random.Range(-radius, radius), 0,
					Random.Range(-radius, radius));
				var point = position + offset;
				var rayOrigin = new Vector3(point.x, rayHeight, point.z);
				var isInRoom = roomTrigger.Collider.bounds.Contains(point);
				if(Physics.Raycast(rayOrigin, Vector3.down, out var hit, 1000f, ~LayerMask.GetMask("RoomTrigger")))
				{
					Debug.DrawLine(rayOrigin, hit.point, Color.yellow, 1010f);
					if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") &&
					   isInRoom) // there was a room check
					{
						foundPoint = true;
						transform.position = point;
					}
				}

				iterations++;
			}

			if(!foundPoint)
			{
				Debug.LogError("Could not find a place to teleport");
			}
		}

		public Vector3 GetCursor()
		{
			return CharacterMover.GetCursor();
		}
		public Entity GetCursorTarget()
		{
			return CharacterMover.GetCursorTarget();
		}
	}
}