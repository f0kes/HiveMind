using System;
using System.Collections.Generic;
using System.Linq;
using GameState;
using Shop;
using UI.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
	public class RollShopUI : MonoBehaviour
	{
		[SerializeField] private Button _rollButton;
		[SerializeField] private int _rollCost;
		[SerializeField] private int _charCost;
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
				var entry = new ShopEntry { CharacterData = characterData, Cost = _charCost };
				entries.Add(entry);
			}
			_shop = new RollShop(entries, _rollCost, _slots.Count);
			Display();
		}
		private void Display()
		{
			var shopDisplay = _shop.GetShop();
			for(var i = 0; i < _slots.Count; i++)
			{
				_slots[i].SetContent(shopDisplay[i].CharacterData);
			}
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