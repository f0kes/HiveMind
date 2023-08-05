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
		[SerializeField] private MinMaxStatRange _duration;

		protected override void PopulateParams()
		{
			AddParam(CS.BlessingMinHealAmount, _minHealAmount);
			AddParam(CS.BlessingLevelIncreaseAmount, _levelIncreaseAmount);
			AddParam(CS.BlessingDuration, _duration);
		}
		protected override void OnActivated()
		{
			base.OnActivated();
			HealEvent.Subscribe(OnHeal);
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			HealEvent.Unsubscribe(OnHeal);
		}

		public override bool IsPermanent()
		{
			return false;
		}

		public override float GetLifetime()
		{
			return GetParam(CS.BlessingDuration);
		}

		private void OnHeal(Combat.Heal obj)
		{
			if(obj.Value < GetParam(CS.BlessingMinHealAmount)) return;
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