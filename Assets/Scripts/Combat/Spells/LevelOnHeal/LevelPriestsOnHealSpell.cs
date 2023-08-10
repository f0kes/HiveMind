using Characters;
using Enums;
using Events.Implementations;
using GameState;
using Stats;
using UnityEngine;

namespace Combat.Spells.LevelOnHeal
{
	public class LevelPriestsOnHealSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _minHealAmount;
		[SerializeField] private MinMaxStatRange _levelIncreaseAmount;
		[SerializeField] private MinMaxStatRange _duration;
		[SerializeField] private MinMaxStatRange _healAmount;

		protected override void PopulateParams()
		{
			AddParam(CS.BlessingMinHealAmount, _minHealAmount);
			AddParam(CS.BlessingLevelIncreaseAmount, _levelIncreaseAmount);
			AddParam(CS.BlessingDuration, _duration);
			AddParam(CS.BlessingHealAmount, _healAmount);
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

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var healValue = GetParam(CS.BlessingHealAmount);
			var friendlyFilter = EntityFilterer.FriendlyFilter.And(EntityFilterer.NotDeadFilter);
			var friendlyCharacters = FilterCharacters(friendlyFilter);
			foreach(var character in friendlyCharacters)
			{
				var healData = new Combat.Heal
				{
					Source = Owner,
					Target = character,
					Value = healValue,
				};
				Debug.Log($"Healing {character.name} for {healValue}");
				BattleProcessor.ProcessHeal(Owner, character, healData);
			}
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