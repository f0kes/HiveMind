using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using GameState;
using Misc;
using Player;
using Shop;
using UI.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
	public class RollShopUI : MonoBehaviour
	{
		[SerializeField] private Button _rollButton;
		[SerializeField] private int _entryCount = 5;
		private List<InventorySlotUI> _slots;
		private RollShop _shop;


		private void Awake()
		{
			_slots = GetComponentsInChildren<InventorySlotUI>().ToList();
			_rollButton.onClick.AddListener(OnRollClicked);
		}
		private void Start()
		{
			var contentDatabase = GameStateController.ContentDatabase;
			var entries = new List<ShopEntry>();

			var rollCost = GameStateController.GameData.RollCost;
			var charCost = GameStateController.GameData.BuyCost;

			foreach(var characterData in contentDatabase.Characters)
			{
				for(var i = 0; i < _entryCount; i++)
				{
					var entry = new ShopEntry { CharacterData = CharacterData.Copy(characterData), Cost = charCost };
					entries.Add(entry);
				}
			} //TODO: move pool generation to RollShop

			_shop = new RollShop(entries, rollCost, _slots.Count, GameStateController.PlayerData.ShopLevel);
			Display();
		}
		private void Display()
		{
			var shopEntries = _shop.GetShop();
			var count = Mathf.Min(_slots.Count, shopEntries.Count);

			for(var i = 0; i < count; i++)
			{
				var shopEntry = shopEntries[i];
				_slots[i].SetActions(() => { }, () => { OnBuy(GameStateController.PlayerData, shopEntry); });
				_slots[i].SetConditions(() => CanDrag(shopEntry), () => false);
				_slots[i].SetContent(shopEntry.CharacterData);
			}
		}
		private TaskResult CanDrag(ShopEntry entry)
		{
			var result = new TaskResult();
			var buyResult = _shop.CanBuy(GameStateController.PlayerData, entry);
			result.Success = buyResult.Success;
			result.Message = buyResult.Message;
			return result;
		}
		private void OnBuy(PlayerData playerData, ShopEntry shopEntry)
		{
			_shop.Buy(playerData, shopEntry);
			TextMessageRenderer.Instance.ShowMessage($"You bought {shopEntry.CharacterData.EntityData.Name}, yor gold is {playerData.Gold}!");
		}
		private void OnRollClicked()
		{
			var result = _shop.Roll(GameStateController.PlayerData);
			if(!result.Success)
			{
				TextMessageRenderer.Instance.ShowMessage(result.Message);
				return;
			}
			Display();
		}
	}
}