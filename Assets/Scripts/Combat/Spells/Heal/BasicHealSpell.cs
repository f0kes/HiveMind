using System;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Settings;
using Enums;
using UnityEngine;

namespace Combat.Spells.Heal
{
	[Serializable]
	public class BasicHealSpell : BaseSpell
	{
		[SerializeField] private SpellParam _healVal;
		[SerializeField] private SpellParam _healInterval;
		[SerializeField] private SpellParam _healDuration;

		protected override void PopulateParams()
		{
			AddParam(CS.Heal, _healVal);
			AddParam(CS.Interval, _healInterval);
			AddParam(CS.Duration, _healDuration);
		}
		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var target = Target as Character.Character;
			if(target == null) return;
			var effect = CreateInstance<BasicHealEffect>();
			target.ApplyNewEffect(GetOwnerCharacter(), this, effect, GetParam(CS.Duration));
		}

		public override CastResult CanCastTarget(Entity target)
		{
			var targetCharacter = target as Character.Character;

			if(targetCharacter == null)
			{
				return new CastResult(CastResultType.Fail, "Target is not a character");
			}
			if(targetCharacter == GetOwnerCharacter())
			{
				return new CastResult(CastResultType.Fail, "Cannot cast on self");
			}

			return EntityFilterer.FilterEntity(Owner, targetCharacter, TeamFilter.Friendly);
		}
		public new static BasicHealSpell CreateDefault()
		{
			var instance = CreateInstance<BasicHealSpell>();
			instance.name = "Basic Heal";
			instance.Level = (int)GameSettings.MaxStatValue;
			instance._healVal = new SpellParam { Min = 10, Max = 20000 };
			instance._healInterval = new SpellParam { Min = 5, Max = 0.01f };
			instance._healDuration = new SpellParam { Min = 1, Max = 100 };
			instance.Behaviour = SpellBehaviour.UnitTarget;
			instance.PopulateParams();
			return instance;
		}
	}
}