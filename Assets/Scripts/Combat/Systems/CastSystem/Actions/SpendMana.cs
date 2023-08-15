using Characters;
using Combat.Spells;

namespace Combat.CastSystem.Actions
{
	public class SpendMana : IAction<BaseSpell>
	{
		public void Execute(BaseSpell spell)
		{
			var character = spell.Owner as Character;
			if(character == null) return;
			character.SpendMana(spell.ManaCost);
		}

	}
}