using UnityEngine;

namespace Stats.Modifiers
{
	
	[CreateAssetMenu(fileName = "New Multiply Modifier", menuName = "Stat Modifier/Multiply")]
	public class StatModifierMultiply : StatModifier
	{
		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat *= Value;
		}
	}
}