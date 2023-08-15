using Combat.ChargeSystem;
using Combat.Spells;

namespace Combat.CastSystem.Actions
{
	public class SpendCharge : IAction<BaseSpell>
	{
		private ICharger _charger;
		public SpendCharge(ICharger charger)
		{
			_charger = charger;
		}
		public void Execute(BaseSpell spell)
		{
			_charger.SetCharge(spell, -1);
		}
	}
}