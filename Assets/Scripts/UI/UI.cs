using System;
using Content;
using DefaultNamespace.Content;
using UnityEngine;

namespace DefaultNamespace.UI
{
	public class UI : MonoBehaviour
	{
		//singleton
		public static UI Instance;
		public ChestUI ChestUI;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}
		public void ShowChestUI(Chest chest)
		{
			ChestUI.Show(chest);
		}
	}
}