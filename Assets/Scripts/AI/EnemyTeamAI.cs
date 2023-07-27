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
		private EntityList _list;
		private void Start()
		{
			_list = GlobalEntities.GetTeam(_teamId);
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
			if(_list.CanCast())
			{
				var list = _list.GetCharacters();
				var randomIndex = UnityEngine.Random.Range(0, list.Count);
				if(randomIndex >= list.Count) return;
				var randomCaster = list[randomIndex];
				randomCaster.SetMana(randomCaster.CurrentMana + 1); //todo: pull from settings 
				if(!randomCaster.ReadyToCast()) return;
				var entitiesInRange = GlobalEntities.GetEntitiesInRange(randomCaster.transform.position, 100f); //todo: get range from spell
				var spell = randomCaster.ActiveSpell;
				var filteredEntities = EntityFilterer.FilterEntitiesWithSpell(randomCaster, entitiesInRange, spell);
				if(filteredEntities.Count > 0)
				{
					var chosen = spell.ChooseBestTarget(filteredEntities);
					randomCaster.CharacterMover.SetCursorTarget(chosen);
					spell.Cast();
				}
			}
		}
		public void SetTeam(EntityList list)
		{
			_list = list;
		}
	}
}