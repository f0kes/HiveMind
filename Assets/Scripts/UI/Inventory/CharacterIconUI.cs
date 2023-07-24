using System;
using Characters;
using Shop;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Inventory
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CharacterIconUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public event Action<PointerEventData> OnBeginDragEvent;
		public event Action<PointerEventData> OnDragEvent;
		public event Action<PointerEventData> OnDropEvent;

		[SerializeField] private Image _icon;
		private Func<bool> _canDrag = () => true;
		private bool _isDragging;
		private CanvasGroup _canvasGroup;

		private void Start()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		public void SetSprite(Sprite sprite)
		{
			_icon.sprite = sprite;
		}

		public void SetDragCondition(Func<bool> canDrag)
		{
			_canDrag = canDrag;
		}
		public void OnBeginDrag(PointerEventData eventData)
		{
			if(!_canDrag()) return;
			OnBeginDragEvent?.Invoke(eventData);
			_isDragging = true;
			_canvasGroup.blocksRaycasts = false;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if(!_isDragging) return;
			OnDragEvent?.Invoke(eventData);
			transform.position = eventData.position;
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if(!_isDragging) return;
			_isDragging = false;
			_canvasGroup.blocksRaycasts = false;
			OnDropEvent?.Invoke(eventData);
		}
	}
}