using UnityEngine;

namespace Stats.Modifiers
{
	[CreateAssetMenu(fileName = "New Multiply Base Positive Modifier", menuName = "Stat Modifier/Multiply Base Positive")]
	public class StatMultiplyBasePositive : StatModifier
	{
		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat += baseValue * (1 + Value) - baseValue;
		}
	}
}