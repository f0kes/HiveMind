using Enums;

namespace Combat.Spells
{
	public static class SpellFilter
	{
		public static bool HasTag(this BaseSpell spell, SpellTag tag)
		{
			return spell.Tags.Contains(tag);
		}
	}
}