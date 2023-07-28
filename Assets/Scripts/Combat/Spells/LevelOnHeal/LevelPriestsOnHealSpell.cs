using Characters;
using Enums;
using Events.Implementations;
using Stats;
using UnityEngine;

namespace Combat.Spells.LevelOnHeal
{
	public class LevelPriestsOnHealSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _minHealAmount;
		[SerializeField] private MinMaxStatRange _levelIncreaseAmount;

		protected override void PopulateParams()
		{
			AddParam(CS.MinHealAmount, _minHealAmount);
			AddParam(CS.BlessingLevelIncreaseAmount, _levelIncreaseAmount);
		}
		protected override void SubscribeToEvents()
		{
			base.SubscribeToEvents();
			HealEvent.Subscribe(OnHeal);
		}

		protected override void UnsubscribeFromEvents()
		{
			base.UnsubscribeFromEvents();
			HealEvent.Unsubscribe(OnHeal);
		}

		private void OnHeal(Combat.Heal obj)
		{
			if(obj.Value < GetParam(CS.MinHealAmount)) return;
			var levelIncreaseAmount = GetParam(CS.BlessingLevelIncreaseAmount);
			var team = GetOwnerCharacter().GetTeam();
			foreach(var entity in team)
			{
				var character = entity as Character;
				if(character == null) continue;
				if(character.Class != CharacterClass.Priest) continue;
				character.LevelUp((int)levelIncreaseAmount);
			}
		}
	}
}