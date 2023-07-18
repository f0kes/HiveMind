using UnityEngine;

namespace Stats.Modifiers
{
	[CreateAssetMenu(fileName = "New Multiply Base Modifier", menuName = "Stat Modifier/Multiply Base")]
	public class StatMultiplyBase : StatModifier
	{
		public override void ApplyMod(ref float finalStat, float baseValue)
		{
			finalStat += (baseValue * Value) - baseValue;
		}
	}
}