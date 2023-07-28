using System.Collections.Generic;
using Characters;
using Enums;
using Stats;
using UnityEngine;

namespace Combat.Spells.DivineBlessing
{
	public class DivineBlessingSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _divineShieldHP;
		[SerializeField] private MinMaxStatRange _divineShieldDuration;

		protected override void PopulateParams()
		{
			AddParam(CS.DivineShieldHP, _divineShieldHP);
			AddParam(CS.DivineShieldDuration, _divineShieldDuration);
		}

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var owner = GetOwnerCharacter();
			if(owner == null) return;
			var targets = owner.GetTeam().GetListCopy();
			foreach(var target in targets)
			{
				var effect = CreateInstance<DivineShieldEffect>();
				target.ApplyNewEffect(owner, this, effect, GetParam(CS.DivineShieldDuration));
			}
		}
	}
}