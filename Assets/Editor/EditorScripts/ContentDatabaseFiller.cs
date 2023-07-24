using System.Collections.Generic;
using Characters;
using Combat.Spells;
using DefaultNamespace.Configs;
using UnityEngine;

namespace Editor.EditorScripts
{
	using UnityEditor;

	[CustomEditor(typeof(ContentDatabase))]
	public class ContentDatabaseFiller : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var container = (ContentDatabase)target;


			if(GUILayout.Button("Auto Fill"))
			{
				FillDatabase(container);
			}
		}

		private void FillDatabase(ContentDatabase container)
		{
			container.Characters = GetAssetsOfType<CharacterData>("Assets/ScriptableObjects/Chars");
			container.Spells = GetAssetsOfType<BaseSpell>("Assets/ScriptableObjects/Spells/");
			
		}
		private List<T> GetAssetsOfType<T>(string path) where T : ScriptableObject
		{
			List<T> assets = new List<T>();
			string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { path });
			foreach(var guid in guids)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
				if(asset != null)
				{
					assets.Add(asset);
				}
			}
			return assets;
		}

	}
}