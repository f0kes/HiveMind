using System.Collections.Generic;
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
		private List<ShopEntry> _pool;
		private List<ShopEntry> _shop;

		private readonly int _rollCost;
		private readonly int _shopSize;

		public RollShop(List<ShopEntry> pool, int rollCost = 2, int shopSize = 6)
		{
			_pool = pool;
			_rollCost = rollCost;
			_shopSize = shopSize;
			_shop = new List<ShopEntry>();
			RollUnconditional();
		}
		public List<ShopEntry> GetShop()
		{
			return _shop;
		}

		public BuyResult Buy(PlayerData player, ShopEntry entry)
		{
			var result = new BuyResult { Entry = entry };
			if(!(_pool.Contains(entry) && _shop.Contains(entry)))
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
			player.Gold -= entry.Cost;

			_shop.Remove(entry);
			_pool.Remove(entry);

			return result;
		}
		public RollResult Roll(PlayerData player)
		{
			var result = new RollResult { Entries = new List<ShopEntry>() };
			if(_pool.Count == 0)
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
			_shop.Clear();
			for(var i = 0; i < _shopSize; i++)
			{
				var entry = _pool[Random.Range(0, _pool.Count)];
				_shop.Add(entry);
				result.Entries.Add(entry);
			}

			return result;
		}
		private void RollUnconditional()
		{
			_shop.Clear();
			for(var i = 0; i < _shopSize; i++)
			{
				var entry = _pool[Random.Range(0, _pool.Count)];
				_shop.Add(entry);
			}
		}
	}
}