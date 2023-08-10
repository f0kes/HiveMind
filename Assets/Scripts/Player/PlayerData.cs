﻿using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using DefaultNamespace;
using Misc;
using UnityEngine;

namespace Player
{
	public class PlayerData
	{
		public Action<int> OnGoldChanged;
		private int _gold;
		private int _maxGold = 35;
		private List<CharacterData> _party;
		private List<CharacterData> _inventory;
		private List<CharacterData> _shopPool;



		public List<CharacterData> Party => _party;
		public List<CharacterData> Inventory => _inventory;
		public List<CharacterData> ShopPool => _shopPool;


		private float _battleLevelPrecise;
		private float _shopLevelPrecise;

		public uint BattleLevel => (uint)Math.Round(_battleLevelPrecise);
		public uint ShopLevel => (uint)Math.Round(_shopLevelPrecise);

		public int LevelsBeaten{get; set;}

		public int CurrentLevel{get; set;}
		public uint NextBattleGoldReward{get; set;}

		public string PlayerName{get; set;} = "Player";

		public int Gold
		{
			get => _gold;
			set => SetGold(value);
		}

		public int MaxGold => _maxGold;

		public PlayerData(int maxGold, int startGold, float battleLevel, float shopLevel,
			List<CharacterData> party = null, List<CharacterData> inventory = null, List<CharacterData> shopPool = null)
		{
			_maxGold = maxGold;
			_gold = startGold;
			_battleLevelPrecise = battleLevel;
			_shopLevelPrecise = shopLevel;

			_party = party ?? new List<CharacterData>();
			_inventory = inventory ?? new List<CharacterData>();
			_shopPool = shopPool ?? new List<CharacterData>();
		}


		public float BattleLevelPrecise
		{
			get => _battleLevelPrecise;
			set => _battleLevelPrecise = value;
		}

		public float ShopLevelPrecise
		{
			get => _shopLevelPrecise;
			set => _shopLevelPrecise = value;
		}

		public void SetParty(List<CharacterData> data)
		{
			_party = data;
		}
		public void AddToPool(CharacterData data)
		{
			_shopPool.Add(data);
		}

		public void AddToParty(CharacterData data)
		{
			_party.Add(data);
		}
		public void RemoveFromParty(CharacterData data)
		{
			_party.Remove(data);
		}
		public void AddToInventory(CharacterData data)
		{
			_inventory.Add(data);
		}
		public void RemoveFromInventory(CharacterData data)
		{
			_inventory.Remove(data);
		}
		public void SetGold(int gold)
		{
			if(gold > _maxGold)
			{
				gold = _maxGold;
			}
			_gold = gold;
			OnGoldChanged?.Invoke(_gold);
		}
		public bool HasCharacter(CharacterData data)
		{
			return _party.Contains(data) || _inventory.Contains(data);
		}
		public TaskResult MoveToShopPool(CharacterData data)
		{
			var container = FindDataContainer(data);
			if(container == null)
				return TaskResult.Failure("Player does not contain this character, cannot move to shop pool");

			_shopPool.Add(data);

			data.SetLevel(ShopLevel);
			container.Remove(data);


			return TaskResult.Success;
		}
		private List<CharacterData> FindDataContainer(CharacterData data)
		{
			if(_party.Contains(data))
				return _party;
			if(_inventory.Contains(data))
				return _inventory;
			return null;
		}

		public void DecrementCooldowns()
		{
			foreach(var characterData in _party)
			{
				characterData.DecrementCooldown();
			}
			foreach(var characterData in _inventory)
			{
				characterData.DecrementCooldown();
			}
			foreach(var characterData in _shopPool)
			{
				characterData.DecrementCooldown();
			}
		}
		public List<string> GetPartyNames()
		{
			return _party.Select(characterData => characterData.EntityData.Name).ToList();
		}
	}
}