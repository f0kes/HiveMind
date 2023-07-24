using System;
using Characters;
using Shop;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Inventory
{
	public class InventorySlotUI : MonoBehaviour
	{
		[SerializeField] private CharacterIconUI _iconPrefab;

		private CharacterIconUI _icon;
		private readonly InventorySlot _slot = new();

		private Func<bool> _dragCondition = () => true;
		private Func<bool> _dropCondition = () => true;

		private Action _onGotIcon;
		private Action _onLostIcon;
		public void SetConditions(Func<bool> dragCondition, Func<bool> dropCondition)
		{
			_dragCondition = dragCondition;
			_dropCondition = dropCondition;
		}
		public void SetActions(Action onGotIcon, Action onLostIcon)
		{
			_onGotIcon = onGotIcon;
			_onLostIcon = onLostIcon;
		}
		public void SetContent(CharacterData data)
		{
			_slot.PutContent(data);
			UpdateIcon();
		}
		public CharacterData GetContent()
		{
			return _slot.GetContent();
		}
		private void UpdateIcon()
		{
			DestroyOldIcon();

			var data = _slot.GetContent();
			if(data == null) return;
			
			var newIcon = Instantiate(_iconPrefab);
			newIcon.SetSprite(data.EntityData.Icon);

			newIcon.OnDragEvent += OnIconDrag;
			newIcon.OnDropEvent += OnIconDrop;


			SetIcon(newIcon);
		}

		private void OnIconDrop(PointerEventData eventData)
		{
			var go = eventData.pointerCurrentRaycast.gameObject;
			InventorySlotUI other = null;
			if(go != null)
			{
				other = go.GetComponent<InventorySlotUI>();
			}
			var result = false;
			if(other != null)
			{
				result = other.CanDrop();
			}
			if(result)
			{
				_slot.MoveContentTo(other._slot);

				_onLostIcon?.Invoke();
				other._onGotIcon?.Invoke();
				other.UpdateIcon();
			}
			UpdateIcon();
		}

		private void OnIconDrag(PointerEventData eventData)
		{
		}

		public bool CanDrop()
		{
			return _dropCondition == null || _dropCondition();
		}
		private void SetIcon(CharacterIconUI icon)
		{
			DestroyOldIcon();
			_icon = icon;
			_icon.SetDragCondition(_dragCondition);
			var rectTransform = GetComponent<RectTransform>();
			var iconTransform = _icon.GetComponent<RectTransform>();

			iconTransform.SetParent(rectTransform, false);
		}
		private void DestroyOldIcon()
		{
			if(_icon == null)
				return;
			_icon.OnDragEvent -= OnIconDrag;
			_icon.OnDropEvent -= OnIconDrop;
			Destroy(_icon.gameObject);
			_icon = null;
		}


		public void ClearContent()
		{
			_slot.PutContent(null);
			DestroyOldIcon();
		}
	}
}