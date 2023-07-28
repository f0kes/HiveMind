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

		protected override void PopulateParams()
		{
			AddParam(CS.EnrageMinDamage, _minDamage);
			AddParam(CS.EnrageLevelUpAmount, _levelUpAmount);
		}
		protected override void SubscribeToEvents()
		{
			base.SubscribeToEvents();
			DamageEvent.Subscribe(OnDamage);
		}

		protected override void UnsubscribeFromEvents()
		{
			base.UnsubscribeFromEvents();
			DamageEvent.Unsubscribe(OnDamage);
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