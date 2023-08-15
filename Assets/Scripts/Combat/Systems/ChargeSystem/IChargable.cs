using Misc;

namespace Combat.Systems.ChargeSystem
{
	public interface IChargable
	{
		float GetMaxCharge();
		float GetChargeGain();
		float GetChargeLoss();
	}
}