using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Combat.Spells;
using UnityEditor;
using UnityEngine;

namespace Editor.EditorScripts
{
	public class GenerateSpellAssets : EditorWindow
	{
		[MenuItem("Game/Generate Spell Assets")]
		public static void Generate()
		{
			var spellTypes = GetEnumerableOfType<BaseSpell>();
			foreach(var spellType in spellTypes)
			{
				if(AssetDatabase.FindAssets($"t:{spellType.Name}").Length != 0) continue;
				var asset = CreateInstance(spellType);
				AssetDatabase.CreateAsset(asset, $"Assets/ScriptableObjects/Spells/{spellType.Name}.asset");
				
			}
		}
		public static IEnumerable<Type> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
		{
			var objects = Assembly.GetAssembly(typeof(T))
				.GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)))
				.ToList();
			return objects;
		}
	}
}