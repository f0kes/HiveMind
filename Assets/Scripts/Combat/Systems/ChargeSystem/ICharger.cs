using System;
using Combat.Systems.ChargeSystem;
using Misc;

namespace Combat.ChargeSystem
{
	public interface ICharger
	{
		public event Action<IChargable> OnChargeStarted; 
		public event Action<IChargable> OnCharged;
		public event Action<IChargable> OnChargeLost;

		TaskResult StartCharging(IChargable chargable);

		TaskResult Charge(IChargable chargable);

		TaskResult StopCharging(IChargable chargable);

		bool Charged(IChargable chargable);

		float GetCharge(IChargable chargable);

		void SetCharge(IChargable chargable, float charge);

		bool ChargeStarted(IChargable chargable);

	}
}