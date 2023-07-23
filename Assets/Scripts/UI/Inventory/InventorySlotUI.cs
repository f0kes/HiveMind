using System;
using Characters;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
	public class InventorySlotUI : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		private CharacterData _content;

		public void SetContent(CharacterData character)
		{
			_content = character;
			_icon.sprite = character.EntityData.Icon;
		}
	}
}