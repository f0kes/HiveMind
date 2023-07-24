using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using GameState;
using UnityEngine;

namespace UI.Inventory
{
	public class PlayerInventoryUI : MonoBehaviour
	{
		[SerializeField] private List<InventorySlotUI> _partySlots;
		[SerializeField] private List<InventorySlotUI> _inventorySlots;

		private void Start()
		{
			RenderContent();
			foreach(var slot in _partySlots)
			{
				slot.SetActions(() => { OnPartyGotIcon(slot); }, () => { OnPartyLostIcon(slot); });
			}
			foreach(var slot in _inventorySlots)
			{
				slot.SetActions(() => { OnInventoryGotIcon(slot); }, () => { OnInventoryLostIcon(slot); });
			}
		}
		private void RenderContent()
		{
			var party = GameStateController.PlayerData.Party;
			var inventory = GameStateController.PlayerData.Inventory;
			InventoryRendererUI.FillSlotList(_partySlots, party);
			InventoryRendererUI.FillSlotList(_inventorySlots, inventory);
		}
		private void OnPartyGotIcon(InventorySlotUI slot)
		{
			GameStateController.PlayerData.AddToParty(slot.GetContent());
		}
		private void OnPartyLostIcon(InventorySlotUI slot)
		{
			GameStateController.PlayerData.RemoveFromParty(slot.GetContent());
		}
		private void OnInventoryGotIcon(InventorySlotUI slot)
		{
			GameStateController.PlayerData.AddToInventory(slot.GetContent());
		}
		private void OnInventoryLostIcon(InventorySlotUI slot)
		{
			GameStateController.PlayerData.RemoveFromInventory(slot.GetContent());
		}
	}
}