using GameState;
using Combat;
using Enums;
using UnityEngine;

namespace Combat.Spells.Heal
{
	public class BasicHealEffect : BaseEffect
	{
		private float _timeToNextTick;
		public override void OnCreated()
		{
			base.OnCreated();
			_timeToNextTick = GetParam(CS.Interval);
		}

		protected override void PopulateParams()
		{
		}

		public override void OnTick(Ticker.OnTickEventArgs obj)
		{
			base.OnTick(obj);
			_timeToNextTick += Ticker.TickInterval;
			if(_timeToNextTick < GetParam(CS.Interval)) return;
			BattleProcessor.ProcessHeal(Owner, Target, new Combat.Heal(Owner, Target, this, GetParam(CS.Heal)));
			_timeToNextTick = 0;
		}
	}
}