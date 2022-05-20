using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
	public class WeaponIcon : MonoBehaviour
	{
		[HideInInspector] public Weapon CurrentWeapon;
		[SerializeField] private Image _image;

		public void SetWeapon(Weapon weapon)
		{
			if (weapon == null) return;
			CurrentWeapon = weapon;
			_image.sprite = weapon.Icon;
			_image.gameObject.SetActive(true);
		}
	}
}