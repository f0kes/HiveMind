using DefaultNamespace;
using UnityEngine;

namespace Combat.Spells
{
	public struct ProjectileData
	{
		public BaseSpell Spell{get; set;}
		public Entity Owner{get; set;}
		public Entity Target{get; set;}
		public float Range{get; set;}
		public Vector3 Velocity{get; set;}
		public Vector3 StartPosition{get; set;}
	}
}