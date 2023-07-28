using UnityEngine;

namespace Stats.Modifiers
{
	[CreateAssetMenu(fileName = "New Add Modifier", menuName = "Stat Modifier/Add")]
	public class StatModifierAdd : StatModifier
	{
		protected override void ApplyModChild(ref float finalStat, float baseValue)
		{
			finalStat += Value;
		}
	}
}