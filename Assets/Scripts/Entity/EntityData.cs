using System;
using System.Collections.Generic;
using Enums;
using Stats;
using UnityEngine;
using Object = UnityEngine.Object;


namespace DefaultNamespace
{
	[Serializable]
	public class EntityData
	{
		public Sprite Icon;
		public string Name;
		public List<EntityTag> Tags = new ();
		public SerializableCharacterStats Stats;
		public uint Level;

		public EntityData()
		{
		}
		public EntityData(EntityData original)
		{
			Icon = original.Icon;
			Name = original.Name;
			Tags = original.Tags;
			Stats = Object.Instantiate(original.Stats);
			Level = original.Level;
		}
		public StatDict<CS> GetStats()
		{
			return Stats.GetStats(Level);
		}
		public void SetLevel(uint level)
		{
			Level = level;
		}

	}
}