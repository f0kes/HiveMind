using System;
using System.Collections.Generic;
using DefaultNamespace;
using Enums;
using Stats.Modifiers;
using UnityEngine;

namespace Combat.Spells.Finisher
{
	[Serializable]
	public class FinisherSpell : BaseSpell
	{
		[SerializeField] private SpellParam _damageBonus;
		[SerializeField] private SpellParam _attackSpeedPenalty;


		protected override void PopulateParams()
		{
			AddParam(CS.DamageBonus, _damageBonus);
			AddParam(CS.AttackSpeedPenalty, _attackSpeedPenalty.Reverse);
		}
		protected override void OnAttachedToCharacter()
		{
			base.OnAttachedToCharacter();
			var atkSpeedPenalty = GetParam(CS.FireRate);
			var modfier = ScriptableObject.CreateInstance<StatModifierMultiply>();
			modfier.Value = atkSpeedPenalty;
			GetOwnerCharacter().Stats.GetStat(CS.FireRate)
				.AddMod(modfier);
		}



		public override CastResult CanCastTarget(Entity target)
		{
			return EntityFilterer.FilterEntity(Owner, target, TeamFilter.Enemy);
		}

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var target = GetCursorTarget();
			if(target == null) return;
			var damageVal = GetOwnerCharacter().Stats[CS.Damage]
			                + (GetParam(CS.Damage) + GetParam(CS.Bonus)) / 2
			                * 1 / target.CurrentHealthPercent;
			var damage = new Damage { Source = Owner, Spell = this, Target = target, Value = damageVal };
			BattleProcessor.ProcessHit(GetOwnerCharacter(), target, damage);
		}
	}
}