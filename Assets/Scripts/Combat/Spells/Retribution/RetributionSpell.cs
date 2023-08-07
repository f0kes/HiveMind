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

		protected override void PopulateParams()
		{
			base.PopulateParams();
			AddParam(CS.RetributionLevelsOnDeath, _levelsOnDeath);
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
				character.LevelUp((int)GetParam(CS.RetributionLevelsOnDeath));
			}
		}
	}
}