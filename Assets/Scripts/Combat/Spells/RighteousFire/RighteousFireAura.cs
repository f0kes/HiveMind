using System.Collections.Generic;
using System.Linq;
using Characters;
using Combat.Spells.DivineBlessing;
using DefaultNamespace;
using Enums;
using GameState;
using Stats;
using Stats.Modifiers;
using UnityEngine;

namespace Combat.Spells.RighteousFire
{
	public class RighteousFireAura : BaseAura
	{
		[SerializeField] private MinMaxStatRange _damageBonus;

		private StatModifierAdd _damageBonusModifier;
		protected override void PopulateParams()
		{
			base.PopulateParams();
			AddParam(CS.DamageBonus, _damageBonus);
		}

		public override void OnCreated()
		{
			base.OnCreated();
			_damageBonusModifier = CreateInstance<StatModifierAdd>();
			_damageBonusModifier.SetValFunc(() => GetParam(CS.DamageBonus));
		}

		public override void OnTick(Ticker.OnTickEventArgs obj)
		{
			base.OnTick(obj);
			var owner = GetOwnerCharacter();
			if(owner == null) return;
			var targets = owner.GetTeam().GetListCopy();
		}

		protected override HashSet<Entity> GetTargets()
		{
			var result = new HashSet<Entity>();
			var owner = GetOwnerCharacter();
			if(owner == null) return result;
			var targets = owner.GetTeam().GetListCopy();
			foreach(var target in
			        from target in targets
			        where target != owner
			        where target.GetEffectOfType<DivineShieldEffect>() != null
			        select target)
			{
				result.Add(target);
			}
			return result;
		}

		protected override void ApplyAura(Entity target)
		{
			target.Stats.GetStat(CS.Damage).AddMod(_damageBonusModifier);
		}

		protected override void RemoveAura(Entity target)
		{
			target.Stats.GetStat(CS.Damage).RemoveMod(_damageBonusModifier);
		}
	}
}