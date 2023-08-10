using Enums;
using Events.Implementations;
using GameState;
using Stats;
using UnityEngine;

namespace Combat.Spells.Retribution
{
	public class RetributionSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _levelsOnDeath;
		[SerializeField] private MinMaxStatRange _initialLevels;

		protected override void PopulateParams()
		{
			base.PopulateParams();
			AddParam(CS.RetributionLevelsOnDeath, _levelsOnDeath);
			AddParam(CS.RetributionInitialLevels, _initialLevels);
		}

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			LevelPaladins((int)GetParam(CS.RetributionInitialLevels));
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			DeathEvent.Subscribe(OnDeathCallback);
		}
		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			DeathEvent.Unsubscribe(OnDeathCallback);
		}
		private void OnDeathCallback(DeathData obj)
		{
			LevelPaladins((int)GetParam(CS.RetributionLevelsOnDeath));
		}
		private void LevelPaladins(int levels)
		{
			var friendlyPaladinsFilter =
				EntityFilterer
					.FriendlyFilter
					.And(EntityFilterer.NotDeadFilter)
					.And(EntityFilterer.ClassFilter(CharacterClass.Paladin));
			var filteredCharacters =
				GameStateController
					.Battle
					.EntityRegistry
					.GetAllCharacters()
					.Filter(Owner, friendlyPaladinsFilter);
			foreach(var character in filteredCharacters)
			{
				character.LevelUp(levels);
			}
		}
	}
}