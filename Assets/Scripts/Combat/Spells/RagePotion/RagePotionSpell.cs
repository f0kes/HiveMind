using Characters;
using DefaultNamespace;
using Enums;
using Stats;
using Stats.Modifiers;
using UnityEngine;

namespace Combat.Spells.RagePotion
{
	//todo: create base potion spell
	public class RagePotionSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _atkSpeedBonus;
		[SerializeField] private MinMaxStatRange _damagePenalty;
		[SerializeField] private MinMaxStatRange _bonusDuration;
		[SerializeField] private MinMaxStatRange _penaltyDuration;

		private StatModifierAdd _atkSpeedBonusMod;
		private StatModifierAdd _damagePenaltyMod;

		protected override void PopulateParams()
		{
			AddParam(CS.RagePotionAtkSpeedBonus, _atkSpeedBonus);
			AddParam(CS.RagePotionDamagePenalty, _damagePenalty);
			AddParam(CS.RagePotionBonusDuration, _bonusDuration);
			AddParam(CS.RagePotionPenaltyDuration, _penaltyDuration);
		}

		public override void OnCreated()
		{
			base.OnCreated();
			_atkSpeedBonusMod = CreateInstance<StatModifierAdd>();
			_damagePenaltyMod = CreateInstance<StatModifierAdd>();

			_atkSpeedBonusMod.SetValFunc(() => GetParam(CS.RagePotionAtkSpeedBonus));
			_damagePenaltyMod.SetValFunc(() => -GetParam(CS.RagePotionDamagePenalty));
		}

		public override CastResult CanCastTarget(Entity target)
		{
			var result = CastResult.Success;
			var ownerCharacter = GetOwnerCharacter();
			if(ownerCharacter == null)
			{
				result.Message = "No owner character";
				result.ResultType = CastResultType.Fail;
				return result;
			}
			var targetCharacter = target as Character;
			if(targetCharacter == null)
			{
				result.Message = "Target is not a character";
				result.ResultType = CastResultType.Fail;
				return result;
			}
			if(targetCharacter == ownerCharacter)
			{
				result.Message = "Can't cast on self";
				result.ResultType = CastResultType.Fail;
				return result;
			}
			if(ownerCharacter.Team != targetCharacter.Team)
			{
				result.Message = "Target is not in owner's team";
				result.ResultType = CastResultType.Fail;
				return result;
			}
			return result;
		}

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var ownerCharacter = GetOwnerCharacter();
			if(ownerCharacter == null) return;
			var targetCharacter = GetCursorTarget();
			if(targetCharacter == null) return;
			var bonusDuration = GetParam(CS.RagePotionBonusDuration);
			var penaltyDuration = GetParam(CS.RagePotionPenaltyDuration);
		}
	}
}