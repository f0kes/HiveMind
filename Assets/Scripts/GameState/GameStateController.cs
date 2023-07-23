using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Battle;
using DefaultNamespace;
using Player;
using UnityEngine;

namespace GameState
{
	public class GameStateController : MonoBehaviour
	{
		public static GameStateController Instance;
		public static GameState CurrentGameState => Instance._currentGameState;
		public static Battle CurrentBattle => Instance._battle;
		public static event Action BattleEnded;

		[SerializeField] private List<Entity> _playerTeamInitial;
		[SerializeField] private List<Entity> _enemyTeamInitial;

		[SerializeField] private InputHandler _inputHandler;
		private InputHandler _inputHandlerInstance;

		private GameState _currentGameState;
		private List<Entity> _playerTeam = new List<Entity>();

		private Battle _battle;
		private EntityFactory _entityFactory;
		public Battle Battle => _battle;

		private void Awake()
		{
			if(Instance == null)
				Instance = this;
			else
			{
				Destroy(gameObject);
				return;
			}
			_playerTeam.AddRange(_playerTeamInitial);
		}
		private void OnDestroy()
		{
			if(Instance == this)
				Instance = null;
		}
		private void Start()
		{
			_inputHandlerInstance = Instantiate(_inputHandler);
			InstantiateLists();
			StartBattle();
		}
		//TODO: remove
		private void InstantiateLists()
		{
			var newPlayerTeam = _playerTeam.Select(Instantiate).ToList();
			_playerTeam = newPlayerTeam;
			var newEnemyTeam = _enemyTeamInitial.Select(Instantiate).ToList();
			_enemyTeamInitial = newEnemyTeam;
			foreach(var entity in _playerTeam)
			{
				entity.gameObject.SetActive(false);
			}
			foreach(var entity in _enemyTeamInitial)
			{
				entity.gameObject.SetActive(false);
			}
		}
		public void StartBattle()
		{
			_currentGameState = GameState.Battle;
			
			_battle = new Battle();
			_entityFactory = new EntityFactory();

			_battle.StartBattle(_playerTeam, _enemyTeamInitial, _entityFactory, _inputHandlerInstance);
			_battle.BattleEnded += OnBattleEnded;
		}

		private void OnBattleEnded(BattleResult obj)
		{
			_battle.BattleEnded -= OnBattleEnded;
			BattleEnded?.Invoke();


			_playerTeam = new List<Entity>();
			if(obj.ResultType == BattleResult.BattleResultType.Win)
			{
				_playerTeam.AddRange(_entityFactory.GetAliveOriginals());
			}
			else
			{
				_playerTeam.AddRange(_playerTeamInitial);
			}
			StartBattle();
		}
	}
}