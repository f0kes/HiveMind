using Characters;
using DefaultNamespace;
using DefaultNamespace.Content;
using UnityEngine;

namespace Content
{
	[RequireComponent(typeof(WeaponContainer))]
	[RequireComponent(typeof(Interactable))]
	public class Chest : MonoBehaviour
	{
		private Interactable _interactable;
		private WeaponContainer _weaponContainer;

		private void Awake()
		{
			_weaponContainer = GetComponent<WeaponContainer>();
			_interactable = GetComponent<Interactable>();
			_interactable.OnInteract += Interact;
		}

		public WeaponContainer GetWeaponContainer()
		{
			return _weaponContainer;
		}

		public Weapon ChooseItem(Weapon weapon)
		{
			var chosenWeapon = _weaponContainer.GetWeapon(weapon);
			if (chosenWeapon != null)
			{
				_weaponContainer.RemoveAllItems();
			}

			return chosenWeapon;
		}


		public void Interact()
		{
			Debug.Log("Interact");
			DefaultNamespace.UI.UI.Instance.ShowChestUI(this);
		}
	}
}