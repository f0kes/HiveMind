using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Inventory
{
	public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private float _hoverTime = 0.01f;
		private float _timeSinceHover;
		private bool _isHovering;
		private Canvas _canvas;
		private void Awake()
		{
			_canvas = FindObjectOfType<Canvas>();
			transform.SetParent(_canvas.transform);
			transform.SetAsLastSibling();
		}
		private async void Start()
		{
			ResetTime();
			await Countdown((int)(_hoverTime * 1000));
			Destroy(gameObject);
		}
		public void SetPosition(Vector3 position)
		{
			var rect = GetComponent<RectTransform>();
			var size = rect.sizeDelta;
			var canvasSize = _canvas.GetComponent<RectTransform>().sizeDelta;
			if (position.x + size.x > canvasSize.x)
			{
				position.x = canvasSize.x - size.x;
			}
			
			
			
			
			transform.position = position;
		}
		public void OnPointerEnter(PointerEventData eventData)
		{
			ResetTime();
			transform.SetAsLastSibling();
		}
		public void OnPointerExit(PointerEventData eventData)
		{
			_isHovering = false;
		}
		public void ResetTime()
		{
			_isHovering = true;
			_timeSinceHover = 0;
		}

		private async UniTask Countdown(int ms)
		{
			while (_timeSinceHover < ms)
			{
				if(Input.anyKeyDown)
				{
					return;
				}
				if(!_isHovering)
				{
					_timeSinceHover += Time.deltaTime * 1000;
				}
				await UniTask.Yield();
			}
		}

	}
}