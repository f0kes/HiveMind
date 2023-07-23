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
	}
}