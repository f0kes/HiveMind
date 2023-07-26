using System;
using Characters;
using Misc;
using Shop;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Inventory
{
	
	public class InventorySlotUI : MonoBehaviour
	{
		[SerializeField] private CharacterIconUI _iconPrefab;
		[SerializeField] private CharacterTooltip _tooltipPrefab;

		private CharacterIconUI _icon;
		private CharacterTooltip _tooltipInstance;
		private readonly InventorySlot _slot = new();

		private Func<TaskResult> _dragCondition = () => true;
		private Func<TaskResult> _dropCondition = () => true;

		private Action _onGotIcon;
		private Action _onLostIcon;
		public void SetConditions(Func<TaskResult> dragCondition, Func<TaskResult> dropCondition)
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
			newIcon.OnPointerEnterEvent += OnIconPointerEnter;
			newIcon.OnPointerExitEvent += OnIconPointerExit;

			SetIcon(newIcon);
		}

		private void OnIconPointerExit(PointerEventData obj)
		{
			if(_tooltipInstance == null) return;
			Debug.Log("OnIconPointerExit");
			_tooltipInstance.Tooltip.OnPointerExit(obj);
		}

		private void OnIconPointerEnter(PointerEventData obj)
		{
			if(_tooltipInstance != null)
			{
				_tooltipInstance.Tooltip.OnPointerEnter(obj);
				return;
			}
			_tooltipInstance = Instantiate(_tooltipPrefab);
			_tooltipInstance.SetData(_slot.GetContent());
			_tooltipInstance.Tooltip.SetPosition(transform.position);
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
				_onLostIcon?.Invoke();
				_slot.MoveContentTo(other._slot);
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
			_icon.OnPointerEnterEvent -= OnIconPointerEnter;
			_icon.OnPointerExitEvent -= OnIconPointerExit;
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