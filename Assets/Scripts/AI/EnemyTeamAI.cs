using System;
using Characters;
using Combat;
using DefaultNamespace;
using Events.Implementations;
using GameState;
using UnityEngine;

namespace AI
{
	public class EnemyTeamAI : MonoBehaviour
	{
		[SerializeField] private ushort _teamId = 1;
		private EntityTeam _team;
		private Character _oldCaster;
		private void Start()
		{
			_team = GameStateController.Battle.EntityRegistry.GetTeam(_teamId);
			SubscribeToEvents();
		}

		private void OnDestroy()
		{
			UnsubscribeFromEvents();
		}
		private void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
		}
		private void UnsubscribeFromEvents()
		{
			Ticker.OnTick -= OnTick;
		}
		private void OnTick(Ticker.OnTickEventArgs obj)
		{
			if(!_team.CanSwap()) return;
			var list = _team.GetCharacters();
			var randomIndex = UnityEngine.Random.Range(0, list.Count);
			if(randomIndex >= list.Count) return;
			var randomCaster = list[randomIndex];

			CharacterSwappedEvent.Invoke(new CharacterSwappedData(_oldCaster, randomCaster));
			_oldCaster = randomCaster;

			if(!randomCaster.ReadyToCast()) return;
			var entitiesInRange =
				GameStateController
					.Battle
					.EntityRegistry
					.GetEntitiesInRange(randomCaster.transform.position, 100f); //todo: get range from spell
			var spell = randomCaster.ActiveSpell;
			var filteredEntities = EntityFilterer.FilterEntitiesWithSpell(randomCaster, entitiesInRange, spell);
			if(filteredEntities.Count > 0)
			{
				var chosen = spell.ChooseBestTarget(filteredEntities);
				randomCaster.CharacterMover.SetCursorTarget(chosen);
				spell.Cast();
			}
		}
		public void SetTeam(EntityTeam team)
		{
			_team = team;
		}
	}
}