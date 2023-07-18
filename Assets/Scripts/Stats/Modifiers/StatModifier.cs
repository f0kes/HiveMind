using UnityEngine;

namespace Stats.Modifiers
{
	public class StatModifier : ScriptableObject
	{
		public float Value;
		public float Priority;

		public string Name => GetType().ToString();


		public virtual void ApplyMod(ref float finalStat, float baseValue)
		{
			//	finalStat = finalStat;
		}

		public override string ToString()
		{
			return Name + ": " + Value;
		}
	}
}