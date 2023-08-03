using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using GameState;
using Misc;
using Player;
using Shop;
using TMPro;
using UI.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Shop
{
	public class RollShopUI : MonoBehaviour
	{
		[SerializeField] private Button _rollButton;
		[SerializeField] private int _entryCount = 5;
		[SerializeField] private TextMeshProUGUI _levelText;

		private List<InventorySlotUI> _slots;
		private RollShop _shop;


		private void Awake()
		{
			_slots = GetComponentsInChildren<InventorySlotUI>().ToList();
			_rollButton.onClick.AddListener(OnRollClicked);
		}
		private void Start()
		{
			var rollCost = GameStateController.GameData.RollCost;
			var charCost = GameStateController.GameData.BuyCost;

			_levelText.text = $"Shop level: {GameStateController.PlayerData.ShopLevel}";
			_shop = new RollShop(
				GameStateController.PlayerData.ShopPool,
				rollCost,
				_slots.Count,
				GameStateController.PlayerData.ShopLevel,
				charCost);
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
			result.IsResultSuccess = buyResult.Success;
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