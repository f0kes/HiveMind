using System;
using Characters;
using Misc;
using Shop;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Inventory
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CharacterIconUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public event Action<PointerEventData> OnBeginDragEvent;
		public event Action<PointerEventData> OnDragEvent;
		public event Action<PointerEventData> OnDropEvent;
		public event Action<PointerEventData> OnPointerEnterEvent;
		public event Action<PointerEventData> OnPointerExitEvent;


		[SerializeField] private Image _icon;
		[SerializeField] private TextMeshProUGUI _cooldownText;
		private Func<TaskResult> _canDrag = () => true;
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
		public void SetCooldown(int cooldown)
		{
			if(cooldown == 0)
			{
				_cooldownText.gameObject.SetActive(false);
				return;
			}
			_cooldownText.gameObject.SetActive(true);
			_cooldownText.text = cooldown.ToString();
		}

		public void SetDragCondition(Func<TaskResult> canDrag)
		{
			_canDrag = canDrag;
		}
		public void OnBeginDrag(PointerEventData eventData)
		{
			var canDrag = _canDrag();
			if(!canDrag)
			{
				TextMessageRenderer.Instance.ShowMessage(canDrag.Message);
				return;
			}
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

		public void OnPointerEnter(PointerEventData eventData)
		{
			OnPointerEnterEvent?.Invoke(eventData);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			OnPointerExitEvent?.Invoke(eventData);
		}
	}
}