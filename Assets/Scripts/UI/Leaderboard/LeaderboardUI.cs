using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FireBase;
using FireBase.Models;
using GameState;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
	[SerializeField] private GameObject _mainPanel;
	[SerializeField] private GameObject _playerEntryContainer;
	[SerializeField] private GameObject _entriesContainer;
	[SerializeField] private Button _newGameButton;

	private List<TextMeshProUGUI> _entriesTexts;
	private TextMeshProUGUI _playerEntryText;

	private void Awake()
	{
		_mainPanel.SetActive(false);
	}
	private void Start()
	{
		GameStateController.Instance.OnGameOver += OnGameOver;
		_entriesTexts = GetChildText(_entriesContainer);
		_playerEntryText = _playerEntryContainer.GetComponentInChildren<TextMeshProUGUI>();
	}

	private async void OnGameOver(GameEntryModel player)
	{
		_mainPanel.SetActive(true);
		_newGameButton.onClick.AddListener(OnNewGameClicked);
		_playerEntryText.text = "Loading...";

		var leaderboard = await GetLeaderBoard();
		if(leaderboard == null)
		{
			_playerEntryText.text = "Error loading leaderboard";
			return;
		}
		leaderboard.Sort();
		_playerEntryText.text = GetEntryText(leaderboard, player);
		for(var i = 0; i < _entriesTexts.Count; i++)
		{
			var entry = leaderboard.GameEntries[i];
			_entriesTexts[i].text = GetEntryText(leaderboard, entry);
		}
	}
	private List<TextMeshProUGUI> GetChildText(GameObject go)
	{
		return go.GetComponentsInChildren<TextMeshProUGUI>().ToList();
	}

	private async void OnNewGameClicked()
	{
		await GameStateController.Instance.LoadShopScene(1000);
	}
	private async Task<LeaderboardModel> GetLeaderBoard()
	{
		if(FirebaseTest.Instance == null)
			return null;
		var leaderboard = await FirebaseTest.Instance.RequestLeaderboard();
		return leaderboard;
	}
	private string GetEntryText(LeaderboardModel leaderboard, GameEntryModel entry)
	{
		var position = leaderboard.GetEntryPosition(entry);
		var nickname = entry.PlayerName;
		var levelsBeaten = entry.LevelsBeaten;
		var text = $"{position}. {nickname} - {levelsBeaten} levels";

		return text;
	}
}