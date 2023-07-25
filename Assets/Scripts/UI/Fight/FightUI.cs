using System;
using System.Collections.Generic;
using Characters;
using FightGeneration;
using GameState;
using TMPro;
using UI.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Fight
{
	public class FightUI : MonoBehaviour
	{
		[SerializeField] private Button _startButton;
		[SerializeField] private List<InventorySlotUI> _enemySlots;
		[SerializeField] private List<InventorySlotUI> _partySlots;
		[SerializeField] private TextMeshProUGUI _levelText;


		private FightGenerator _fightGenerator;

		private List<CharacterData> _party;
		private List<CharacterData> _enemies;

		private bool _initialized;
		private void Start()
		{
			UpdateParty();
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
			_startButton.onClick.AddListener(StartBattle);
			_initialized = true;
		}
		private void OnEnable()
		{
			if(!_initialized) return;
			UpdateParty();
			RenderContent();
		}
		private void UpdateParty()
		{
			_party = GameStateController.PlayerData.Party;
		}
		private void StartBattle()
		{
			GameStateController.Instance.StartBattle(_party, _enemies);
		}
		private void InitFightGenerator()
		{
			var pool = GameStateController.ContentDatabase.Characters;
			_fightGenerator = new FightGenerator(pool, _enemySlots.Count, GameStateController.PlayerData.BattleLevel);
			_enemies = _fightGenerator.Generate();
		}
		private void RenderContent()
		{
			_levelText.text = $"Level: {GameStateController.PlayerData.BattleLevel}";
			InventoryRendererUI.FillSlotList(_partySlots, _party);
			InventoryRendererUI.FillSlotList(_enemySlots, _enemies);
		}
	}
}