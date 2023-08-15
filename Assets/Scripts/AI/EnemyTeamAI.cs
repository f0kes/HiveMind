using System;
using System.Linq;
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
		private Character _chosenCaster;
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
			if(_chosenCaster == null || _chosenCaster.IsDead)
				ChooseCaster();
			if(_chosenCaster == null) return;
			Charge(_chosenCaster);
			DischargeOthers(_chosenCaster);
			var target = ChooseTarget(_chosenCaster);
			if(target == null) return;
			var castReady = _chosenCaster.ReadyToCast();
			if(!castReady)
			{
				Debug.LogError(castReady.Message);
				return;
			}
			var result = Cast(_chosenCaster);
			if(result) _chosenCaster = null;
		}

		private void Charge(Character chosenCaster)
		{
			if(chosenCaster.ActiveSpell == null) return;
			GameStateController.ChargeSystem.Charge(chosenCaster.ActiveSpell);
		}
		private void DischargeOthers(Character chosenCaster)
		{
			var list = _team.GetCharacters();
			foreach(var character in list)
			{
				if(character == chosenCaster) continue;
				GameStateController.ChargeSystem.StopCharging(character.ActiveSpell);
			}
		}
		private Entity ChooseTarget(Character caster)
		{
			var entitiesInRange =
				GameStateController
					.Battle
					.EntityRegistry
					.GetEntitiesInRange(caster.transform.position, 100f); //todo: get range from spell

			var spell = caster.ActiveSpell;
			var filteredEntities = EntityFilterer.FilterEntitiesWithSpell(caster, entitiesInRange, spell);
			if(filteredEntities.Count <= 0) return null;
			var chosen = spell.ChooseBestTarget(filteredEntities);
			caster.CharacterMover.SetCursorTarget(chosen);
			return chosen;
		}
		private static bool Cast(Character randomCaster)
		{
			GameStateController.CastSystem.Invoke(randomCaster.ActiveSpell);
			return true;
		}
		private void ChooseCaster()
		{
			if(!_team.CanSwap()) return;
			var list = _team.GetCharacters().Where(c => c.ReadyToCharge()).ToList();
			var randomIndex = UnityEngine.Random.Range(0, list.Count);
			_chosenCaster = randomIndex >= list.Count ? null : list[randomIndex];
			if(_chosenCaster == null) return;
			CharacterSwappedEvent.Invoke(new CharacterSwappedData(_oldCaster, _chosenCaster));
			_oldCaster = _chosenCaster;
		}
		public void SetTeam(EntityTeam team)
		{
			_team = team;
		}
	}
}