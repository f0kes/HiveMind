using System;
using System.Collections.Generic;
using Characters;
using Combat.Battle;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Configs;
using Misc;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameState
{
	public class GameStateController : MonoBehaviour
	{
		public static GameStateController Instance{get; private set;}

		[SerializeField] private uint _goldPerBattle = 10; //TODO: load from config

		[SerializeField] private SceneField _battleScene;
		[SerializeField] private SceneField _shopScene;

		[SerializeField] private ContentDatabase _contentDatabase;
		[SerializeField] private GameObject _singletonsPrefab;
		public static ContentDatabase ContentDatabase => Instance._contentDatabase;


		private PlayerData _playerData;
		public static PlayerData PlayerData => Instance._playerData;


		private Battle _battle;
		private CharacterFactory _characterFactory;


		private void Awake()
		{
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
			_playerData = new PlayerData { Gold = 100 }; //TODO: load from config
		}
		public async void StartBattle(IEnumerable<CharacterData> party, IEnumerable<CharacterData> enemies)
		{
			await SceneManager.LoadSceneAsync(_battleScene);
			
			Instantiate(_singletonsPrefab);
			
			_characterFactory = new CharacterFactory();
			_battle = new Battle();

			var partyEntities = _characterFactory.Create(party, 0);
			var enemyEntities = _characterFactory.Create(enemies, 1);

			_battle.StartBattle(partyEntities, enemyEntities);
			_battle.BattleEnded += OnBattleEnded;
		}

		private async void OnBattleEnded(BattleResult battleResult)
		{
			await SceneManager.LoadSceneAsync(_shopScene);

			_battle.BattleEnded -= OnBattleEnded;
			GlobalEntities.DestroyDeadEntities();
			GlobalEntities.Clear();
			if(battleResult.ResultType == BattleResult.BattleResultType.Win)
			{
				var playerParty = _characterFactory.GetAliveOriginals(0);
				_playerData.SetParty(playerParty);
				_playerData.Gold += (int)_goldPerBattle;
				_playerData.BattleLevel += 1; //TODO: load from config
			}
			else
			{
				ResetPlayerData();
			}
		}
	}
}