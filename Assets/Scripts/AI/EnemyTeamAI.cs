using System;
using Characters;
using Combat;
using DefaultNamespace;
using GameState;
using UnityEngine;

namespace AI
{
	public class EnemyTeamAI : MonoBehaviour
	{
		[SerializeField] private ushort _teamId = 1;
		private EntityTeam _team;
		private void Start()
		{
			_team = EntityList.GetTeam(_teamId);
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
			if(_team.CanCast())
			{
				var list = _team.GetCharacters();
				var randomIndex = UnityEngine.Random.Range(0, list.Count);
				var randomCaster = list[randomIndex];
				if(!randomCaster.ReadyToCast()) return;
				var entitiesInRange = EntityList.GetEntitiesInRange(randomCaster.transform.position, 100f); //todo: get range from spell
				var spell = randomCaster.Spell;
				var filteredEntities = EntityFilterer.FilterEntitiesWithSpell(randomCaster, entitiesInRange, spell); //todo: euristic
				if(filteredEntities.Count > 0)
				{
					var randomTarget = filteredEntities[UnityEngine.Random.Range(0, filteredEntities.Count)];
					randomCaster.CharacterMover.SetCursorTarget(randomTarget);
					spell.Cast();
				}
			}
		}
		public void SetTeam(EntityTeam team)
		{
			_team = team;
		}
	}
}