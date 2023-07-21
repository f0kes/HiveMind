using UnityEngine;

namespace VFX
{
	public class SpellIcon : VFXEffect
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;
		public void SetIcon(Sprite icon)
		{
			if(icon == null) return;
			_spriteRenderer.sprite = icon;
		}
	}
}