using System.Collections.Generic;
using System.Linq;
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

		private static void FillDatabase(ContentDatabase container)
		{
			container.Characters = GetAssetsOfType<CharacterData>("Assets/ScriptableObjects/Chars");
			container.Spells = GetAssetsOfType<BaseSpell>("Assets/ScriptableObjects/Spells/");
			//save changes

			EditorUtility.SetDirty(container);
			AssetDatabase.SaveAssets();
		}
		private static List<T> GetAssetsOfType<T>(string path) where T : ScriptableObject
		{
			var guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { path });
			return guids
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<T>)
				.Where(asset => asset != null)
				.ToList();
		}

	}
}