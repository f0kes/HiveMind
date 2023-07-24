using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UI.SideBar
{
	public class SideBar : MonoBehaviour
	{
		[Serializable]
		public struct SideBarEntry
		{
			public Button Button;
			public GameObject Menu;
		}
		[SerializeField] private SideBarEntry[] _entries;
		
		private void Awake()
		{
			foreach(var entry in _entries)
			{
				entry.Button.onClick.AddListener(() => OnButtonClicked(entry.Button));
			}
		}

		private void OnButtonClicked(Object entryButton)
		{
			foreach(var entry in _entries)
			{
				var button = entry.Button;
				var menu = entry.Menu;
				menu.SetActive(button == entryButton);
			}
		}
	}
}