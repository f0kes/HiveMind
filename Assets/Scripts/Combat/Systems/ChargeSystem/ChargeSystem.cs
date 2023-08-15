using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Combat.Battle;
using Combat.CastSystem;
using Combat.ChargeSystem;
using Combat.Spells;
using GameState;
using Misc;
using UnityEngine;
using VFX;

namespace Combat.Systems.ChargeSystem
{
	public class ChargeSystem : BattleSystem, ICharger, ISpellInvoker
	{
		public event Action<IChargable> OnChargeStarted;
		public event Action<IChargable> OnCharged;
		public event Action<IChargable> OnBreak;
		public event Action<IChargable> OnChargeLost;
		private class ChargeData
		{
			public float CurrentCharge;
			public bool Charging;
			public bool Charged;
			public bool ChargeStarted;
		}
		private readonly Dictionary<IChargable, ChargeData> _charges = new();
		private List<IRequirement<BaseSpell>> _castRequirements = new();
		private List<IAction<BaseSpell>> _castActions = new();
		private List<IAction<BaseSpell>> _onChargeLostActions = new();
		public ChargeSystem(IBattle battle) : base(battle)
		{
		}

		public override void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
			OnBreak += BreakAction;
		}

		public override void UnsubscribeFromEvents()
		{
			Ticker.OnTick -= OnTick;
			OnBreak -= BreakAction;
		}

		private void BreakAction(IChargable obj)
		{
			foreach(var action in _onChargeLostActions)
			{
				var spell = obj as BaseSpell;
				if(spell == null) continue;
				action.Execute(spell);
			}
		}

		public void AddOnChargeLostAction(IAction<BaseSpell> action)
		{
			_onChargeLostActions.Add(action);
		}
		private void OnTick(Ticker.OnTickEventArgs obj)
		{
			foreach(var (chargable, charge) in _charges.Where(c => c.Value.ChargeStarted))
			{
				if(charge.Charging)
				{
					charge.CurrentCharge += chargable.GetChargeGain() * Ticker.TickInterval;
				}
				else
				{
					charge.CurrentCharge -= chargable.GetChargeLoss() * Ticker.TickInterval;
				}

				if(charge.CurrentCharge >= chargable.GetMaxCharge() && !charge.Charged)
				{
					OnCharged?.Invoke(chargable);
				}

				else if(charge.CurrentCharge <= 0)
				{
					charge.ChargeStarted = false;
					charge.Charging = false;
					OnBreak?.Invoke(chargable);
				}
				if(charge.CurrentCharge < chargable.GetMaxCharge() && charge.Charged)
				{
					OnChargeLost?.Invoke(chargable);
				}
				charge.Charged = charge.CurrentCharge >= chargable.GetMaxCharge();
			}
		}


		public TaskResult StartCharging(IChargable chargable)
		{
			if(chargable is BaseSpell spell)
			{
				return Invoke(spell);
			}
			return TaskResult.Failure("Chargable is not a spell");
		}

		public TaskResult Charge(IChargable chargable)
		{
			var result = TaskResult.Failure("");
			var spell = chargable as BaseSpell;
			if(!this[chargable].ChargeStarted && spell != null)
			{
				result = Invoke(spell);
				if(result)
				{
					this[chargable].CurrentCharge = 0;
				}
			}
			if(this[chargable].ChargeStarted)
			{
				this[chargable].Charging = true;
			}

			return result;
		}

		public TaskResult StopCharging(IChargable chargable)
		{
			if(chargable == null) return TaskResult.Failure("Chargable is null");
			this[chargable].Charging = false;
			return TaskResult.Success;
		}

		public bool Charged(IChargable chargable)
		{
			return this[chargable].Charged;
		}

		public float GetCharge(IChargable chargable)
		{
			return this[chargable].CurrentCharge;
		}

		public void SetCharge(IChargable chargable, float charge)
		{
			this[chargable].CurrentCharge = charge;
		}

		public bool ChargeStarted(IChargable chargable)
		{
			return this[chargable].ChargeStarted;
		}

		public TaskResult CanCharge(BaseSpell chargable)
		{
			return CanInvoke(chargable);
		}

		public void AddRequirement(IRequirement<BaseSpell> castRequirement)
		{
			_castRequirements.Add(castRequirement);
		}

		public void AddAction(IAction<BaseSpell> castAction)
		{
			_castActions.Add(castAction);
		}

		public TaskResult Invoke(BaseSpell spell)
		{
			if(this[spell].ChargeStarted) return TaskResult.Failure("Spell is already charging");
			var result = CanInvoke(spell);
			if(!result) return result;
			this[spell].ChargeStarted = true;
			OnChargeStarted?.Invoke(spell);
			SpawnVFX(spell);
			foreach(var action in _castActions)
			{
				action.Execute(spell);
			}
			return result;
		}
		private void SpawnVFX(BaseSpell spell)
		{
			VFXSystem.I.PlayEffectFollow(VFXSystem.Data.SpellStartEffect, spell.Owner.transform);
			var follow = VFXSystem.I.PlayEffectFollow(VFXSystem.Data.SpellChargeEffect, spell.Owner.transform);
			OnBreak += chargable =>
			{
				if(chargable is not BaseSpell baseSpell) return;
				if(baseSpell == spell)
				{
					follow.Stop();
				}
			};
		}

		public TaskResult CanInvoke(BaseSpell spell)
		{
			var result = TaskResult.Success;
			if(spell == null) return TaskResult.Failure("Spell is null");
			foreach(var requirement in _castRequirements)
			{
				result = requirement.DoesSatisfy(spell);
				if(!result)
				{
					return result;
				}
			}
			return result;
		}

		private ChargeData this[IChargable chargable]
		{
			get
			{
				if(!_charges.ContainsKey(chargable))
				{
					_charges.Add(chargable, new ChargeData());
				}
				return _charges[chargable];
			}
			set => _charges[chargable] = value;
		}
	}
}