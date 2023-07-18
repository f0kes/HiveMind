using Combat.Spells;
using DefaultNamespace;

namespace Combat
{
	public struct Damage
	{
		public Damage(Entity source, Entity target, BaseSpell spell, float value)
		{
			Source = source;
			Target = target;
			Spell = spell;
			Value = value;
		}

		public Entity Source;
		public Entity Target;
		public BaseSpell Spell;
		public float Value;
	}
	public struct Heal
	{
		public Heal(Entity source, Entity target, BaseSpell spell, float value)
		{
			Source = source;
			Target = target;
			Spell = spell;
			Value = value;
		}
		public Entity Source;
		public Entity Target;
		public BaseSpell Spell;
		public float Value;
	}
}