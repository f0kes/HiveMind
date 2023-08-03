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
using Events.Implementations;
using GameState;
using MapGeneration.Rooms;
using Misc;
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
		private CharacterData _characterData;
		private Character _swapTarget;
		private int _currentMana;

		public CharacterControlsProvider ControlsProvider;
		public CharacterMover CharacterMover{get; private set;}
		public CharacterShooter CharacterShooter{get; private set;}
		public CharacterInteractor CharacterInteractor{get; private set;}

		private List<BaseSpell> _spells = new List<BaseSpell>(); //TODO: move to entity
		private BaseSpell _activeSpell;

		public BaseSpell ActiveSpell => _activeSpell;
		public float AIDesirability => _characterData.AIDesirability;
		public float AIThreat => _characterData.AIThreat;
		public int MaxMana => _activeSpell != null ? _activeSpell.ManaCost : 0;
		public int CurrentMana => _currentMana;
		public CharacterClass Class => _characterData.Class;

		public static Character FromData(CharacterData data)
		{
			var result = Instantiate(data.Prefab);
			result.Init(data);
			return result;
		}
		public void Init(CharacterData data)
		{
			_characterData = CharacterData.Copy(data);
			SetData(_characterData.EntityData);
			_currentMana = 0;
			InitSpells();
		}
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
		protected override void ChildStart()
		{
			base.ChildStart();
			SubscribeToEvents();
		}
		private void OnDestroy()
		{
			UnSubscribeFromEvents();
			DestroySpells();
		}
		private void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
		}

		private void UnSubscribeFromEvents()
		{
			Ticker.OnTick -= OnTick;
		}
		public void TakeFatigue(FatigueEventData obj)
		{
			SetMaxLevel((int)(MaxLevel - obj.FatigueValue));
			SetLevel((int)(Level - obj.FatigueValue));
		}
		private void OnTick(Ticker.OnTickEventArgs obj)
		{
		}
		public void InitSpells()
		{
			if(_characterData.Spells == null)
			{
				Debug.LogError("Spells are null");
				return;
			}

			if(_characterData.Spells.Count == 0)
			{
				var heal = BasicHealSpell.CreateDefault();
				InitSpell(heal);
			}
			foreach(var copy in _characterData.Spells)
			{
				Debug.Log(copy.Behaviour);
				InitSpell(copy);
			}

			var attack = AutoAttackSpell.CreateDefault();
			InitSpell(attack);

			foreach(var spell in _spells)
			{
				var activeSpellSet = false;
				switch(spell.Behaviour)
				{
					case SpellBehaviour.Default:
						break;
					case SpellBehaviour.Passive:
						break;
					case SpellBehaviour.UnitTarget:
						_activeSpell = spell;
						activeSpellSet = true;
						break;
					case SpellBehaviour.PointTarget:
						_activeSpell = spell;
						activeSpellSet = true;
						break;
					case SpellBehaviour.Active:
						_activeSpell = spell;
						activeSpellSet = true;
						break;
					default:
						break;
				}
				if(activeSpellSet)
				{
					break;
				}
			}
		}

		public void InitSpell(BaseSpell spell)
		{
			spell = Instantiate(spell);
			spell.SetOwner(this);
			spell.OnCreated();
			_spells.Add(spell);
		}
		public void DestroySpells()
		{
			foreach(var spell in _spells)
			{
				DestroySpell(spell);
			}
			_spells.Clear();
		}
		public void DestroySpell(BaseSpell spell)
		{
			spell.OnDestroyed();
			Destroy(spell);
		}


		public void TeleportTeam(RoomTrigger roomTrigger, float radius = 1.8f, float rayHeight = 10f)
		{
			List<Entity> team = GameStateController.Battle.EntityRegistry.GetEntitiesOnTeam(Team);
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
		public void SpendMana(int manaCost)
		{
			SetMana(CurrentMana - manaCost);
		}
		public void SetMana(int characterCurrentMana)
		{
			characterCurrentMana = Mathf.Clamp(characterCurrentMana, 0, MaxMana);
			_currentMana = characterCurrentMana;
			Events.ManaChanged?.Invoke(characterCurrentMana);
		}
		public TaskResult ReadyToCast(BaseSpell spell = null)
		{
			if(spell == null)
			{
				spell = ActiveSpell;
			}
			var result = new TaskResult { IsResultSuccess = true };
			if(IsDead)
			{
				result.IsResultSuccess = false;
				result.Message = "Character is dead";
				return result;
			}
			if(spell == null)
			{
				result.IsResultSuccess = false;
				result.Message = "No spell selected";
				return result;
			}
			if(spell.ManaCost > CurrentMana)
			{
				result.IsResultSuccess = false;
				result.Message = "Not enough mana";
				return result;
			}
			return result;
		}


	}
}