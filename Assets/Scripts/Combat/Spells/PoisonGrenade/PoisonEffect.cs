using Enums;
using GameState;
using UnityEngine;

namespace Combat.Spells.PoisonGrenade
{
	//create asset menu

	public class PoisonEffect : BaseEffect
	{

		protected override void PopulateParams()
		{
		}

		public override void OnTick(Ticker.OnTickEventArgs obj)
		{
			base.OnTick(obj);
			var damage = new Damage { Source = Owner, Target = Target, Spell = this, Value = GetParam(CS.DOT) / Ticker.TickInterval };
			Target.TakeDamage(damage);
		}
	}
}