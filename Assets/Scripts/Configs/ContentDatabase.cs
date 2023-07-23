using System.Collections.Generic;
using Characters;
using Combat.Spells;
using UnityEngine;

namespace DefaultNamespace.Configs
{
	[CreateAssetMenu(fileName = "ContentDatabase", menuName = "Configs/ContentDatabase")]
	public class ContentDatabase : ScriptableObject
	{
		public List<CharacterData> Characters;
		public List<BaseSpell> Spells;
	}
}