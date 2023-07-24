using System;
using System.Collections.Generic;
using Combat.Spells;
using DefaultNamespace;
using UnityEngine;

namespace Characters
{
	[CreateAssetMenu(fileName = "CharacterData", menuName = "Characters/CharacterData")]
	public class CharacterData : ScriptableObject
	{
		public Character Prefab;
		public EntityData EntityData;
		public List<BaseSpell> Spells;
		public float AIDesirability = 1;
		public float AIThreat = 1;

		public static CharacterData Copy(CharacterData original)
		{
			var result = CreateInstance<CharacterData>();
			result.Prefab = original.Prefab;
			result.EntityData = new EntityData(original.EntityData);
			result.Spells = new List<BaseSpell>(original.Spells);
			result.AIDesirability = original.AIDesirability;
			result.AIThreat = original.AIThreat;
			return result;
		}
	}
}