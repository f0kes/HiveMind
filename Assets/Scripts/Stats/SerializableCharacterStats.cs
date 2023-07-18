using System;
using System.Collections.Generic;
using Enums;
using Stats;
using Stats.Structures;
using UnityEditor;
using UnityEngine;

namespace Stats
{
	[Serializable]
	[CreateAssetMenu(fileName = "CharacterStats", menuName = "Stats/CharacterStats")]
	public class SerializableCharacterStats : ScriptableObject
	{
		[Serializable]
		public struct EnumeratedStat
		{
			public CS Name;
			public Stat Value;
		}
		[SerializeField] private List<EnumeratedStat> enumeratedStats;
		public StatDict<CS> GetStats()
		{
			var stats = new StatDict<CS>();
			foreach (var stat in enumeratedStats)
			{
				stats.SetStat(stat.Name, stat.Value);
			}
			return stats;
		}
	}
}