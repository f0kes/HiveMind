using System.Collections.Generic;
using System.Linq;
using Characters;
using DefaultNamespace;
using Player;
using UnityEngine;

namespace Shop
{
	public struct BuyResult
	{
		public bool Success;
		public ShopEntry Entry;
		public string Message;
	}
	public struct RollResult
	{
		public bool Success;
		public List<ShopEntry> Entries;
		public string Message;
	}
	public class RollShop
	{
		private List<CharacterData> _pool;

		private List<ShopEntry> _entryPool;
		private List<ShopEntry> _shop;

		private readonly int _rollCost;
		private readonly int _shopSize;
		private readonly uint _shopLevel;
		private readonly int _charCost;


		public RollShop(List<CharacterData> pool, int rollCost = 2, int shopSize = 6, uint shopLevel = 1, int charCost = 3)
		{
			_pool = pool;
			_entryPool = pool
				.Select(characterData => new ShopEntry
				{
					CharacterData = characterData,
					Cost = charCost
				})
				.ToList();

			_charCost = charCost;
			_rollCost = rollCost;
			_shopSize = shopSize;
			_shopLevel = shopLevel;
			_shop = new List<ShopEntry>();
			SetLevels(_entryPool);
			RollUnconditional();
		}
		private void RemoveFromPool(ShopEntry entry)
		{
			_entryPool.Remove(entry);
			_pool.Remove(entry.CharacterData);
		}
		public void MoveShopToPool()
		{
			AddRangeToPool(_shop);
			_shop.Clear();
		}
		private void AddRangeToPool(IReadOnlyCollection<ShopEntry> entries)
		{
			_entryPool.AddRange(entries);
			_pool.AddRange(entries.Select(entry => entry.CharacterData));
		}
		public List<ShopEntry> GetShop()
		{
			return _shop;
		}
		private void SetLevels(List<ShopEntry> entries)
		{
			foreach(var entry in entries)
			{
				entry.CharacterData.EntityData.Level = _shopLevel;
			}
		}
		public BuyResult CanBuy(PlayerData player, ShopEntry entry)
		{
			var result = new BuyResult { Entry = entry };
			if(!_shop.Contains(entry))
			{
				result.Success = false;
				result.Message = "Entry not found in shop";
				return result;
			}
			if(player.Gold < entry.Cost)
			{
				result.Success = false;
				result.Message = "Not enough gold";
				return result;
			}
			result.Success = true;
			return result;
		}
		public BuyResult Buy(PlayerData player, ShopEntry entry)
		{
			var result = CanBuy(player, entry);
			if(!result.Success) return result;
			player.Gold -= entry.Cost;

			_shop.Remove(entry);

			return result;
		}
		public RollResult Roll(PlayerData player)
		{
			var result = new RollResult { Entries = new List<ShopEntry>() };
			if(_entryPool.Count == 0)
			{
				result.Success = false;
				result.Message = "Pool is empty";
				return result;
			}
			if(player.Gold < _rollCost)
			{
				result.Success = false;
				result.Message = "Not enough gold";
				return result;
			}

			player.Gold -= _rollCost;
			result.Success = true;
			result.Entries = RollUnconditional();

			return result;
		}
		private List<ShopEntry> RollUnconditional()
		{
			var result = new List<ShopEntry>();
			AddRangeToPool(_shop);
			_shop.Clear();
			var count = Mathf.Min(_entryPool.Count, _shopSize);
			for(var i = 0; i < count; i++)
			{
				var entry = _entryPool[Random.Range(0, _entryPool.Count)];
				RemoveFromPool(entry);
				_shop.Add(entry);
				result.Add(entry);
			}
			return result;
		}
	}
}