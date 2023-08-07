using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Combat;
using Combat.Battle;
using Combat.Spells;
using Cysharp.Threading.Tasks;
using DefaultNamespace.Configs;
using Events;
using FireBase;
using FireBase.Models;
using Misc;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameState
{
	public class GameStateController : MonoBehaviour
	{
		public struct StartBattleResult
		{
			public bool Success;
			public string ErrorMessage;
		}

		public event Action<GameEntryModel> OnGameOver;

		[SerializeField] private GameData _gameData;

		[SerializeField] private SceneField _battleScene;
		[SerializeField] private SceneField _shopScene;

		[SerializeField] private ContentDatabase _contentDatabase;
		[SerializeField] private GameObject _singletonsPrefab;


		private uint _goldPerBattle;

		private PlayerData _playerData;
		private GameEntryModel _currentGameEntry = new GameEntryModel();

		private List<IBattleSystem> _combatSystems = new List<IBattleSystem>();
		private Battle _battle;
		private ICharacterFactory _characterFactory;
		private ProjectileSystem _projectileSystem;
		private ActivatorSystem _activatorSystem;
		private FatigueSystem _fatigueSystem;
		private ManaSystem _manaSystem;

		public static GameStateController Instance{get; private set;}
		public static ActivatorSystem ActivatorSystem => Instance._activatorSystem;
		public static PlayerData PlayerData => Instance._playerData;
		public static GameData GameData => Instance._gameData;
		public static ContentDatabase ContentDatabase => Instance._contentDatabase;

		public static Battle Battle => Instance._battle;
		public static ICharacterFactory CharacterFactory => Instance._characterFactory;
		public static ProjectileSystem ProjectileSystem => Instance._projectileSystem;


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
				return;
			}

			_contentDatabase.Init();
			_currentGameEntry = new GameEntryModel();
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
			_playerData = new PlayerData(_gameData.MaxGold,
				(int)_gameData.StartingGold,
				_gameData.StartingBattleLevel,
				_gameData.StartingShopLevel,
				shopPool: _contentDatabase.GenerateCharacterPool(_gameData.ShopRepeats, ContentDatabase.Purchasable));
		}
		public void SetPlayerName(string playerName)
		{
			_playerData.PlayerName = playerName;
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
			if(party.Any(d => !d.CanUse()))
			{
				return new StartBattleResult
				{
					Success = false,
					ErrorMessage = "Party has unusable characters"
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

			_currentGameEntry.OnHeroesUse(party);

			Instantiate(_singletonsPrefab);

			_characterFactory = new CharacterFactory();
			_battle = new Battle();


			InitBattleSystems(_battle);

			var partyEntities = _characterFactory.Create(party, 0);
			var enemyEntities = _characterFactory.Create(enemies, 1);

			_battle.StartBattle(partyEntities, enemyEntities);
			_fatigueSystem.Start();

			_battle.BattleEnded += OnBattleEnded;
		}
		private void InitBattleSystems(IBattle battle)
		{
			_fatigueSystem = new FatigueSystem(
				battle, //TODO: create a pool class for this
				_gameData.TimeToStartFatigue,
				_gameData.FatigueTickTime,
				_gameData.FatigueStartValue,
				_gameData.FatigueIncrement);
			_combatSystems.Add(_fatigueSystem);

			_manaSystem = new ManaSystem(battle, _gameData.ManaPerSwap);
			_combatSystems.Add(_manaSystem);

			_activatorSystem = new ActivatorSystem(battle);
			_combatSystems.Add(_activatorSystem);

			_projectileSystem = new ProjectileSystem(battle);
			_combatSystems.Add(_activatorSystem);


			foreach(var system in _combatSystems)
			{
				system.Start();
			}
		}

		private async void OnBattleEnded(BattleResult battleResult)
		{
			const int winDelay = 1000;
			const int loseDelay = 1000;

			HandleRoundEnd(battleResult);
			var delay = battleResult.ResultType == BattleResult.BattleResultType.Win ? winDelay : loseDelay; //TODO: fix this
			if(battleResult.ResultType == BattleResult.BattleResultType.Win) //todo: fix this
			{
				await LoadShopScene(delay);
			}

			_battle.BattleEnded -= OnBattleEnded;
			StopCombatSystems();
			Ticker.ResetEvents();
		}
		private void HandleRoundEnd(BattleResult battleResult)
		{
			if(battleResult.ResultType == BattleResult.BattleResultType.Win)
			{
				_playerData.DecrementCooldowns();
				foreach(var data in _playerData.Party)
				{
					data.LaunchCooldown();
				}
				var playerDead = _characterFactory.QueryOriginals(x => x.Team == 0 && x.IsDead);
				foreach(var dead in playerDead)
				{
					_playerData.MoveToShopPool(dead);
				}
				_playerData.SetGold(_playerData.Gold + (int)_goldPerBattle);
				ProgressLevels();
				_playerData.LevelsBeaten = _playerData.CurrentLevel;
			}
			else
			{
				ResetGame();
			}
		}
		private void ResetGame()
		{
			_currentGameEntry.PlayerName = _playerData.PlayerName;
			_currentGameEntry.LevelsBeaten = _playerData.LevelsBeaten;
			_currentGameEntry.SetLastParty(_playerData.Party);
			if(FirebaseTest.Instance != null)
			{
				FirebaseTest.Instance.PushGameEntry(_currentGameEntry);
			}
			OnGameOver?.Invoke(_currentGameEntry);
			ResetPlayerData();
			_currentGameEntry = new GameEntryModel();
		}
		private void ProgressLevels()
		{
			_playerData.BattleLevelPrecise += 1 * GameData.LevelsPerBattle;
			_playerData.ShopLevelPrecise += GameData.EnemyToPlayerLevelScaling.ReverseValue * GameData.LevelsPerBattle;
			_playerData.CurrentLevel += 1;
		}
		private void StopCombatSystems()
		{
			foreach(var combatSystem in _combatSystems)
			{
				combatSystem.Stop();
			}
			_combatSystems.Clear();
		}

		public async UniTask LoadShopScene(int delay)
		{
			await UniTask.Delay(delay);
			await SceneManager.LoadSceneAsync(_shopScene);
		}
	}
}