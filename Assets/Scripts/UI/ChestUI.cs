using System;
using Content;
using DefaultNamespace.Content;
using Player;
using UnityEngine;

namespace DefaultNamespace.UI
{
	public class ChestUI : MonoBehaviour
	{
		[SerializeField] private WeaponContainerUI _chestContainer;
		[SerializeField] private WeaponContainerUI _inventoryContainer;
		
		private bool _active;

		public void Show(Chest chest)
		{
			gameObject.SetActive(true);
			_chestContainer.ShowWeaponContainer(chest.GetWeaponContainer());
			_chestContainer.gameObject.SetActive(true);
			InputHandler.Instance.DisableInputs();
		}

		public void Hide()
		{
			gameObject.SetActive(false);
			_chestContainer.gameObject.SetActive(false);
			InputHandler.Instance.EnableInputs();
		}

		private void Update()
		{
			if (!_active) return;
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Hide();
			}
		}
	}
}