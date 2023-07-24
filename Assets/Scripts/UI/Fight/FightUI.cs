using System;
using System.Collections.Generic;
using Characters;
using FightGeneration;
using GameState;
using UI.Inventory;
using UnityEngine;

namespace UI.Fight
{
	public class FightUI : MonoBehaviour
	{
		[SerializeField] private List<InventorySlotUI> _enemySlots;
		[SerializeField] private List<InventorySlotUI> _partySlots;

		[SerializeField] private uint _enemyLevel = 10; //todo: move to config

		private FightGenerator _fightGenerator;
		private List<CharacterData> _enemies;
		private bool _initialized;
		private void Start()
		{
			InitFightGenerator();
			RenderContent();
			foreach(var slot in _partySlots)
			{
				slot.SetConditions(() => false, () => false);
			}
			foreach(var slot in _enemySlots)
			{
				slot.SetConditions(() => false, () => false);
			}
			_initialized = true;
		}
		private void OnEnable()
		{
			if(!_initialized) return;
			RenderContent();
		}
		private void InitFightGenerator()
		{
			var pool = GameStateController.ContentDatabase.Characters;
			_fightGenerator = new FightGenerator(pool, _enemySlots.Count, _enemyLevel);
			_enemies = _fightGenerator.Generate();
		}
		private void RenderContent()
		{
			var party = GameStateController.PlayerData.Party;
			var enemies = _enemies;

			InventoryRendererUI.FillSlotList(_partySlots, party);
			InventoryRendererUI.FillSlotList(_enemySlots, enemies);
		}
	}
}