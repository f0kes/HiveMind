using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
	public class ManaSegment : MonoBehaviour
	{
		[FormerlySerializedAs("Image")]
		[SerializeField] private Image MainImage;
		[SerializeField] private Image OutlineImage;

		private Color _mainColor;
		private Color _outlineColor;

		private float _currentGlow = 1f;
		private void Awake()
		{
			_mainColor = MainImage.color;
			_outlineColor = OutlineImage.color;

			MainImage.material = Instantiate(MainImage.material);
			OutlineImage.material = Instantiate(OutlineImage.material);

			MainImage.material.color = _mainColor;
			OutlineImage.material.color = _outlineColor;
		}
		public void Enable()
		{
			MainImage.enabled = true;
			OutlineImage.enabled = true;
		}
		public void Disable()
		{
			MainImage.enabled = false;
			OutlineImage.enabled = false;
		}
		public Color GetInitialColor()
		{
			return _mainColor;
		}
		private void UpdateColor()
		{
			MainImage.material.color = _mainColor * _currentGlow;
			OutlineImage.material.color = _outlineColor * _currentGlow;
		}
		public void SetColor(Color color)
		{
			MainImage.material.color = color * _currentGlow;
		}
		public void Glow(float val)
		{
			_currentGlow = val;
			UpdateColor();
		}
		public void DisableGlow()
		{
			_currentGlow = 1f;
			UpdateColor();
		}
	}
}