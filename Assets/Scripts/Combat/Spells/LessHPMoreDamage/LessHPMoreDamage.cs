using Characters;
using DefaultNamespace;
using Enums;
using Stats;
using UnityEngine;
using VFX;

namespace Combat.Spells.LessHPMoreDamage
{
	public class LessHPMoreDamage : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _damagePerMissingHealth;

		protected override void PopulateParams()
		{
			AddParam(CS.ExecuteDamagePerMissingHP, _damagePerMissingHealth);
		}

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var target = Target as Character;
			var owner = GetOwnerCharacter();
			if(target == null) return;
			if(owner == null) return;
			var missingHP = owner.MaxHealth - owner.CurrentHealth;
			var damageVal = missingHP * GetParam(CS.ExecuteDamagePerMissingHP);

			var damage = new Damage
			{
				Source = owner,
				Target = target,
				Spell = this,
				Value = damageVal
			};

			BattleProcessor.ProcessHit(damage);
			VFXSystem.I.PlayEffectFollow(VFXSystem.Data.ExecuteEffect, target.transform);
		}

		public override CastResult CanCastTarget(Entity target)
		{
			var result = CastResult.Fail;
			if(target == null)
			{
				result.Message = "No target";
				return result;
			}
			var targetCharacter = target as Character;
			if(targetCharacter == null)
			{
				result.Message = "Target is not a character";
				return result;
			}
			if(targetCharacter.IsDead)
			{
				result.Message = "Target is dead";
				return result;
			}
			if(targetCharacter == GetOwnerCharacter())
			{
				result.Message = "Cannot cast on self";
				return result;
			}
			result = EntityFilterer.FilterEntity(Owner, targetCharacter, EntityFilterer.EnemyFilter);
			return result;
		}
	}
}