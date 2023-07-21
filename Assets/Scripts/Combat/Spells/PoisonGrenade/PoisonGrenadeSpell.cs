using System;
using System.Collections.Generic;
using DefaultNamespace;
using Enums;
using Stats;
using Stats.Structures;
using UnityEngine;

namespace Combat.Spells.PoisonGrenade
{
	[Serializable]
	public class PoisonGrenadeSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _dotDamage;
		[SerializeField] private MinMaxStatRange _dotDuration;
		[SerializeField] private MinMaxStatRange _splashRadius;
		[SerializeField] private MinMaxStatRange _splashDamagePercent;

		protected override void PopulateParams()
		{
			AddParam(CS.DOT, _dotDamage);
			AddParam(CS.Duration, _dotDuration);
			AddParam(CS.Radius, _splashRadius);
			AddParam(CS.SplashDamage, _splashDamagePercent);
		}

		public override void OnBulletHit(Entity target)
		{
			base.OnBulletHit(target);
			var spashDamage = GetParam(CS.SplashDamage) * Stats[CS.Damage];
			var damage = new Damage { Spell = this, Value = spashDamage };
			var targets = EntityFinder.FindEntitiesInRadius(target.transform.position, GetParam(CS.Radius));
			foreach(var splashTarget in targets)
			{
				BattleProcessor.ProcessHit(Owner, splashTarget, damage);
			}
		}

		public override void OnHitLanded(Entity target)
		{
			base.OnHitLanded(target);
			var poisonEffect = CreateInstance<PoisonEffect>();
			var duration = GetParam(CS.Duration) * 0.01f;
			target.ApplyNewEffect(Owner, this, poisonEffect, duration);
		}

	}
}