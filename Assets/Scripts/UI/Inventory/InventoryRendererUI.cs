using System;
using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace UI.Inventory
{
	[Serializable]
	public class InventoryRendererUI
	{
		[SerializeField] private List<InventorySlotUI> _slots;
		private List<CharacterData> _data;
		public void SetData(List<CharacterData> data)
		{
			_data = data;
			FillSlotList(_slots, _data);
		}
		public List<InventorySlotUI> GetSlots()
		{
			return _slots;
		}
		public static void FillSlotList(IReadOnlyList<InventorySlotUI> slots, IReadOnlyList<CharacterData> data)
		{
			for(var i = 0; i < slots.Count; i++)
			{
				if(i < data.Count)
				{
					slots[i].SetContent(data[i]);
				}
				else
				{
					slots[i].ClearContent();
				}
			}
		}
	}
}