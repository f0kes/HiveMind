using Combat.Battle;
using Combat.CastSystem.Actions;
using Combat.CastSystem.Requirements;
using Combat.ChargeSystem;
using Combat.Spells.GenericStun;
using Combat.Systems.Activator;

namespace Combat.CastSystem
{
	public static class CastSystemFactory
	{
		public static CastSystem CreateDefault(IBattle battle, ICharger charger, IActivator activator)
		{
			var castSystem = new CastSystem(battle, activator);
			castSystem.AddRequirement(new AliveRequirement());
			//castSystem.AddRequirement(new ManaRequirement());
			castSystem.AddRequirement(new ChargedRequirement(charger));
			castSystem.AddRequirement(new EffectRequirement<GenericStunEffect>(false));

			castSystem.AddAction(new SpendCharge(charger));
			//castSystem.AddRequirement(new UseRequirement());
			return castSystem;
		}
		public static void AddChargeRequirements(this Systems.ChargeSystem.ChargeSystem system)
		{
			system.AddRequirement(new AliveRequirement());
			system.AddRequirement(new ManaRequirement());
			system.AddRequirement(new UseRequirement());
			system.AddRequirement(new EffectRequirement<GenericStunEffect>(false));

			system.AddOnChargeLostAction(new SpendMana());
		}
	}
}