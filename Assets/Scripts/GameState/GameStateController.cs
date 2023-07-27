using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Combat;
using Combat.Battle;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Configs;
using Events;
using Misc;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameState
{
	public class GameStateController : MonoBehaviour
	{
		[SerializeField] private GameData _gameData;
		public struct StartBattleResult
		{
			public bool Success;
			public string ErrorMessage;
		}
		public static GameStateController Instance{get; private set;}

		private uint _goldPerBattle;

		[SerializeField] private SceneField _battleScene;
		[SerializeField] private SceneField _shopScene;

		[SerializeField] private ContentDatabase _contentDatabase;
		[SerializeField] private GameObject _singletonsPrefab;
		public static ContentDatabase ContentDatabase => Instance._contentDatabase;


		private PlayerData _playerData;
		public static PlayerData PlayerData => Instance._playerData;
		public static GameData GameData => Instance._gameData;


		private List<ICombatSystem> _combatSystems = new List<ICombatSystem>();
		private Battle _battle;
		private CharacterFactory _characterFactory;
		private FatigueSystem _fatigueSystem;
		private ManaSystem _manaSystem;


		private void Awake()
		{
			_gameData = Instantiate(_gameData);
			_goldPerBattle = _gameData.GoldPerBattle;
			if(Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
			ResetPlayerData();
		}
		private void OnDestroy()
		{
			if(Instance == this)
			{
				Instance = null;
			}
		}
		private void ResetPlayerData()
		{
			_playerData = new PlayerData
			{
				Gold = (int)GameData.StartingGold,
				ShopLevelPrecise = GameData.StartingShopLevel,
				BattleLevelPrecise = GameData.StartingBattleLevel,
			};
		}
		public StartBattleResult TryStartBattle(List<CharacterData> party, List<CharacterData> enemies)
		{
			if(party.Count == 0)
			{
				return new StartBattleResult
				{
					Success = false,
					ErrorMessage = "Party is empty"
				};
			}
			if(enemies.Count == 0)
			{
				return new StartBattleResult
				{
					Success = false,
					ErrorMessage = "Enemies are empty"
				};
			}
			StartBattle(party, enemies);
			return new StartBattleResult
			{
				Success = true
			};
		}
		private async void StartBattle(List<CharacterData> party, List<CharacterData> enemies)
		{
			await SceneManager.LoadSceneAsync(_battleScene);

			Instantiate(_singletonsPrefab);

			_characterFactory = new CharacterFactory();
			_battle = new Battle();

			InitCombatSystems();

			var partyEntities = _characterFactory.Create(party, 0);
			var enemyEntities = _characterFactory.Create(enemies, 1);

			_battle.StartBattle(partyEntities, enemyEntities);
			_fatigueSystem.Start();

			_battle.BattleEnded += OnBattleEnded;
		}
		private void InitCombatSystems()
		{
			_fatigueSystem = new FatigueSystem(
				GlobalEntities.GetAllCharacters(), //TODO: create a pool class for this
				_gameData.TimeToStartFatigue,
				_gameData.FatigueTickTime,
				_gameData.FatigueStartValue,
				_gameData.FatigueIncrement);

			_manaSystem = new ManaSystem(_gameData.ManaPerSwap);

			_combatSystems.Add(_fatigueSystem);
			_combatSystems.Add(_manaSystem);

			foreach(var system in _combatSystems)
			{
				system.Start();
			}
		}

		private async void OnBattleEnded(BattleResult battleResult)
		{
			await SceneManager.LoadSceneAsync(_shopScene);

			_battle.BattleEnded -= OnBattleEnded;
			StopCombatSystems();
			GlobalEntities.DestroyDeadEntities();
			GlobalEntities.Clear();
			EventResetter.Reset();

			if(battleResult.ResultType == BattleResult.BattleResultType.Win)
			{
				var playerParty = _characterFactory.GetAliveOriginals(0);
				_playerData.SetParty(playerParty);
				_playerData.Gold += (int)_goldPerBattle;
				_playerData.BattleLevelPrecise += GameData.EnemyToPlayerLevelScaling.Value;
				_playerData.ShopLevelPrecise += 1;
			}
			else
			{
				ResetPlayerData();
			}
		}
		private void StopCombatSystems()
		{
			foreach(var combatSystem in _combatSystems)
			{
				combatSystem.Stop();
			}
			_combatSystems.Clear();
		}
	}
}