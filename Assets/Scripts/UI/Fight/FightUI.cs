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
		[SerializeField] private Button _skipButton;

		[SerializeField] private List<InventorySlotUI> _enemySlots;
		[SerializeField] private List<InventorySlotUI> _partySlots;

		[SerializeField] private TextMeshProUGUI _levelText;
		[SerializeField] private TextMeshProUGUI _nextBattleGoldText;


		private FightGenerator _fightGenerator;

		private List<CharacterData> _party;
		private List<CharacterData> _enemies;

		private bool _initialized;
		private void Start()
		{
			UpdateParty();
			Refresh();
			foreach(var slot in _partySlots)
			{
				slot.SetConditions(() => false, () => false);
			}
			foreach(var slot in _enemySlots)
			{
				slot.SetConditions(() => false, () => false);
			}
			_startButton.onClick.AddListener(StartBattle);
			_skipButton.onClick.AddListener(SkipBattle);
			_initialized = true;
		}

		private void SkipBattle()
		{
			var result = GameStateController.Instance.TrySkipBattle();
			if(!result)
			{
				TextMessageRenderer.Instance.ShowMessage(result.Message);
				return;
			}
			Refresh();
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
			var result = GameStateController.Instance.TryStartBattle(_party, _enemies);
			if(!result.Success)
			{
				TextMessageRenderer.Instance.ShowMessage(result.ErrorMessage);
				return;
			}
		}
		private void Refresh()
		{
			var pool = GameStateController.ContentDatabase.Characters;
			_fightGenerator = new FightGenerator(pool, _enemySlots.Count, GameStateController.PlayerData.BattleLevel);
			_enemies = _fightGenerator.Generate();
			RenderContent();
		}
		private void RenderContent()
		{
			_levelText.text = $"Level: {GameStateController.PlayerData.BattleLevel}";
			_nextBattleGoldText.text = $"Yo will get {GameStateController.NextBattleGoldReward} gold";
			InventoryRendererUI.FillSlotList(_partySlots, _party);
			InventoryRendererUI.FillSlotList(_enemySlots, _enemies);
		}
	}
}