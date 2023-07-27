using System;
using Misc;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	public class DamageFill : MonoBehaviour
	{
		[SerializeField] private float _fadeTime = 0.5f;
		[SerializeField] private float _distToTravel = 2f;
		[SerializeField] [ColorUsage(true, true)]
		private Color _startColor = new Color(1.05f, 1.05f, 1.05f, 1.05f);
		[SerializeField] [ColorUsage(true, true)]
		private Color _targetColor = new Color(0f, 0f, 0f, 0f);
		private RectTransform _rectTransform;
		private Image _image;

		private float _timer;



		private void Awake()
		{
			_rectTransform = GetComponent<RectTransform>();
			_image = GetComponent<Image>();
		}
		private void Start()
		{
			_image.color = _startColor;
		}
		
		private void Update()
		{
			_timer += Time.deltaTime;
			if(_timer > _fadeTime)
			{
				Destroy(gameObject);
			}

			else
			{
				var blend = Tween.BezierBlend(_timer / _fadeTime);
				_rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, _distToTravel * -blend);
				_image.color = Color.Lerp(_startColor, _targetColor, blend);
			}
		}
	}
}