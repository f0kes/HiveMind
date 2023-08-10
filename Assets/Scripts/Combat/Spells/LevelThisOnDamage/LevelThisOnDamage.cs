using Characters;
using Enums;
using Events.Implementations;
using Stats;
using UnityEngine;

namespace Combat.Spells.LevelThisOnDamage
{
	public class LevelThisOnDamage : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _minDamage;
		[SerializeField] private MinMaxStatRange _levelUpAmount;
		[SerializeField] private MinMaxStatRange _duration;
		[SerializeField] private MinMaxStatRange _initialLevelUpAmount;

		protected override void PopulateParams()
		{
			AddParam(CS.EnrageMinDamage, _minDamage);
			AddParam(CS.EnrageLevelUpAmount, _levelUpAmount);
			AddParam(CS.EnrageDuration, _duration);
			AddParam(CS.EnrageInitialLevelUpAmount, _initialLevelUpAmount);
		}
		protected override void OnActivated()
		{
			base.OnActivated();
			DamageEvent.Subscribe(OnDamage);
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			DamageEvent.Unsubscribe(OnDamage);
		}

		public override bool IsPermanent()
		{
			return false;
		}

		public override float GetLifetime()
		{
			return GetParam(CS.EnrageDuration);
		}

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var levelUpAmount = GetParam(CS.EnrageInitialLevelUpAmount);
			GetOwnerCharacter().LevelUp((int)levelUpAmount);
		}

		private void OnDamage(Damage obj)
		{
			if(obj.Value < GetParam(CS.EnrageMinDamage)) return;
			var character = obj.Target as Character;
			if(character == null || character.Class != CharacterClass.Warrior) return;
			var levelUpAmount = GetParam(CS.EnrageLevelUpAmount);
			GetOwnerCharacter().LevelUp((int)levelUpAmount);
		}
	}
}