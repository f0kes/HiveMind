using Characters;
using Combat.Spells.GenericStun;
using Enums;
using Events.Implementations;
using GameState;
using Stats;
using UnityEngine;

namespace Combat.Spells.Hexquake
{
	public class HexquakeSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _stunDuration;
		protected override void PopulateParams()
		{
			base.PopulateParams();
			AddParam(CS.HexquakeStunDuration, _stunDuration);
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
			var targetCharacter = obj.Target as Character;
			if(targetCharacter == null)
			{
				return;
			}
			if(targetCharacter.Class != CharacterClass.Warlock) return;
			
			var enemyFilter =
				EntityFilterer.EnemyFilter
					.And(EntityFilterer.CharacterFilter);

			var filteredCharacters =
				GameStateController
					.Battle
					.EntityRegistry
					.GetAllCharacters()
					.Filter(Owner, enemyFilter);
			foreach(var character in filteredCharacters)
			{
				var effect = CreateInstance<GenericStunEffect>();
				character.ApplyNewEffect(GetOwnerCharacter(), this, effect, GetParam(CS.HexquakeStunDuration));
			}
		}
	}
}