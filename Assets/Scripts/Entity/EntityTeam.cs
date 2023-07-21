﻿using System.Collections;
using System.Collections.Generic;
using Characters;
using Combat.Spells;
using DefaultNamespace.Settings;
using GameState;
using Stats.Structures;
using UnityEngine;

namespace DefaultNamespace
{
	public class EntityTeam : IEnumerable<Entity>
	{
		private ushort _teamId;
		private readonly List<Entity> _list = new();
		public Stat SwapCooldown = new(GameSettings.SwapCooldown);
		public Stat CastCooldown = new(GameSettings.SwapCooldown);
		private float _swapCooldownTimer;
		private float _castCooldownTimer;
		private bool _canCast;
		public EntityTeam(ushort teamId)
		{
			_teamId = teamId;
			SubscribeToEvents();
		}

		~EntityTeam()
		{
			UnsubscribeFromEvents();
		}
		private void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
		}
		private void UnsubscribeFromEvents()
		{
			Ticker.OnTick -= OnTick;
		}
		private void SubscribeToEntityEvents(Entity entity)
		{
			entity.Events.SpellCasted += OnCast;
			entity.Events.SwappedWithCharacter += OnSwap;
		}
		private void UnsubscribeFromEntityEvents(Entity entity)
		{
			entity.Events.SpellCasted -= OnCast;
			entity.Events.SwappedWithCharacter -= OnSwap;
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
			return _swapCooldownTimer >= SwapCooldown;
		}
		private void OnSwap(Character character)
		{
			_swapCooldownTimer = 0;
			_canCast = true;
		}
		private void OnCast(BaseSpell spell)
		{
			_castCooldownTimer = 0;
			_canCast = false;
		}
		public void Add(Entity entity)
		{
			_list.Add(entity);
			SubscribeToEntityEvents(entity);
		}
		public void Remove(Entity entity)
		{
			_list.Remove(entity);
			UnsubscribeFromEntityEvents(entity);
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