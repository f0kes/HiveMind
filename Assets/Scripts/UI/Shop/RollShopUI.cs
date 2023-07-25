using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using GameState;
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
		[SerializeField] private int _rollCost; //TODO: move to game settings
		[SerializeField] private int _charCost;
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
			foreach(var characterData in contentDatabase.Characters)
			{
				for(var i = 0; i < _entryCount; i++)
				{
					var entry = new ShopEntry { CharacterData = CharacterData.Copy(characterData), Cost = _charCost };
					entries.Add(entry);
				}
			}
			_shop = new RollShop(entries, _rollCost, _slots.Count);
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
		private bool CanDrag(ShopEntry entry)
		{
			return _shop.CanBuy(GameStateController.PlayerData, entry).Success;
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