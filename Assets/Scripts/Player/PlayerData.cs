using System;
using System.Collections.Generic;
using Characters;
using DefaultNamespace;

namespace Player
{
	public class PlayerData
	{
		public Action<int> OnGoldChanged;
		private int _gold;
		private List<CharacterData> _party = new List<CharacterData>();
		private List<CharacterData> _inventory = new List<CharacterData>();

		public int Gold
		{
			get => _gold;
			set
			{
				OnGoldChanged?.Invoke(value);
				_gold = value;
			}
		}

		public List<CharacterData> Party => _party;
		public List<CharacterData> Inventory => _inventory;
		private float _battleLevelPrecise;
		private float _shopLevelPrecise;

		public uint BattleLevel => (uint)Math.Round(_battleLevelPrecise);
		public uint ShopLevel => (uint)Math.Round(_shopLevelPrecise);

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

	}
}