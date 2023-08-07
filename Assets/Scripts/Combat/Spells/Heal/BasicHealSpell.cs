using System;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Settings;
using Enums;
using Stats;
using UnityEngine;

namespace Combat.Spells.Heal
{
	[Serializable]
	public class BasicHealSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _healVal;
		[SerializeField] private MinMaxStatRange _healInterval;
		[SerializeField] private MinMaxStatRange _healDuration;

		protected override void PopulateParams()
		{
			AddParam(CS.Heal, _healVal);
			AddParam(CS.Interval, _healInterval);
			AddParam(CS.Duration, _healDuration);
		}
		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var target = Target as Characters.Character;
			if(target == null) return;
			
			var effect = CreateInstance<BasicHealEffect>();
			target.ApplyNewEffect(GetOwnerCharacter(), this, effect, GetParam(CS.Duration));
		}

		public override CastResult CanCastTarget(Entity target)
		{
			var targetCharacter = target as Characters.Character;

			if(targetCharacter == null)
			{
				return new CastResult(CastResultType.Fail, "Target is not a character");
			}
			if(targetCharacter == GetOwnerCharacter())
			{
				return new CastResult(CastResultType.Fail, "Cannot cast on self");
			}
			if(targetCharacter.IsDead)
			{
				return new CastResult(CastResultType.Fail, "Target is dead");
			}

			return EntityFilterer.FilterEntity(Owner, targetCharacter, EntityFilterer.FriendlyFilter);
		}
		public new static BasicHealSpell CreateDefault()
		{
			var instance = CreateInstance<BasicHealSpell>();
			instance.name = "Basic Heal";
			instance.Level = (int)GameSettings.MaxStatValue / 10;
			instance._healVal = new MinMaxStatRange(40, 8000);
			instance._healInterval = new MinMaxStatRange(1f, 0.1f); //{ Min = 0.5f, Max = 0.01f };
			instance._healDuration = new MinMaxStatRange(5, 100); //{ Min = 1, Max = 100 };
			instance.Behaviour = SpellBehaviour.UnitTarget;
			instance.PopulateParams();
			return instance;
		}
	}
}