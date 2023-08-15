using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using Combat.Spells;
using DefaultNamespace.Settings;
using Events.Implementations;
using GameState;
using Stats.Structures;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{

	public class EntityTeam : IEnumerable<Entity>
	{
		public event Action<EntityTeam> OnTeamWiped;

		private ushort _teamId;
		private readonly List<Entity> _list = new();
		public Stat SwapCooldown = new(GameSettings.SwapCooldown);
		public Stat CastCooldown = new(GameSettings.SwapCooldown);
		private float _swapCooldownTimer;
		private float _castCooldownTimer;
		private bool _canCast;
		public int Count => _list.Count;
		public EntityTeam(ushort teamId)
		{
			_teamId = teamId;
			SubscribeToEvents();
			Reset();
		}


		~EntityTeam()
		{
			UnsubscribeFromEvents();
		}
		private void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
			CharacterSwappedEvent.Subscribe(OnSwap);
		}
		private void UnsubscribeFromEvents()
		{
			Ticker.OnTick -= OnTick;
			CharacterSwappedEvent.Unsubscribe(OnSwap);
		}

		private void OnSwap(CharacterSwappedData obj)
		{
			if(obj.NewCharacter.Team != _teamId) return;
			_swapCooldownTimer = 0;
			_canCast = true;
		}

		public void SetList<T>(List<T> list) where T : Entity
		{
			Clear();
			foreach(var entity in list)
			{
				Add(entity);
			}
		}
		public void Clear()
		{
			_list.Clear();
		}
		public void Reset()
		{
			_swapCooldownTimer = SwapCooldown;
			_castCooldownTimer = SwapCooldown;
		}
		private void OnTick(Ticker.OnTickEventArgs obj)
		{
			_swapCooldownTimer += Ticker.TickInterval;
			_castCooldownTimer += Ticker.TickInterval;
		}
		public Entity GetRandom()
		{
			return _list[Random.Range(0, _list.Count)];
		}
		public Character GetRandomCharacter()
		{
			var chars = _list.FindAll(entity => entity is Character);
			return (Character)chars[Random.Range(0, chars.Count)];
		}
		public int GetCount()
		{
			return _list.Count;
		}
		public bool IsPlayerTeam()
		{
			return _teamId == 0;
		}
		public bool CanCast()
		{
			if(IsPlayerTeam())
			{
				return _canCast;
			}
			return _castCooldownTimer >= CastCooldown;
		}
		public bool CanSwap()
		{
			//return _swapCooldownTimer >= SwapCooldown;
			return true;
		}

		public void Add(Entity entity)
		{
			_list.Add(entity);
		}
		public void Remove(Entity entity)
		{
			_list.Remove(entity);
			if(_list.Count == 0)
			{
				OnTeamWiped?.Invoke(this);
			}
		}
		public IEnumerator<Entity> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		public List<Entity> GetListCopy()
		{
			return new List<Entity>(_list);
		}
		public List<Character> GetCharacters()
		{
			var casters = new List<Character>();
			foreach(var entity in _list)
			{
				if(entity is Character character)
				{
					casters.Add(character);
				}
			}
			return casters;
		}

		public float GetSwapCooldown()
		{
			return SwapCooldown - _swapCooldownTimer;
		}

		public float GetCastCooldown()
		{
			return CastCooldown - _castCooldownTimer;
		}
	}
}