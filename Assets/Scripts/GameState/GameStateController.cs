using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Battle;
using DefaultNamespace;
using DefaultNamespace.Configs;
using Player;
using UnityEngine;

namespace GameState
{
	public class GameStateController : MonoBehaviour
	{
		public static GameStateController Instance{get; private set;}
		[SerializeField] private List<Entity> _playerTeamInitial;
		[SerializeField] private List<Entity> _enemyTeamInitial;
		[SerializeField] private ContentDatabase _contentDatabase;
		private PlayerData _playerData;


		public static ContentDatabase ContentDatabase => Instance._contentDatabase;
		public static PlayerData PlayerData => Instance._playerData;
		private List<Entity> _playerTeam = new List<Entity>();

		private Battle _battle;
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
			_playerTeam.AddRange(_playerTeamInitial);
			ResetPlayerData();
		}
		private void OnDestroy()
		{
			if(Instance == this)
			{
				Instance = null;
			}
		}
		private void Start()
		{
			//StartBattle();
		}
		private void ResetPlayerData()
		{
			_playerData = new PlayerData { Gold = 100 }; //TODO: load from config
		}
		public void StartBattle()
		{
			_battle = new Battle();
			_battle.StartBattle(_playerTeam, _enemyTeamInitial);
			_battle.BattleEnded += OnBattleEnded;
		}

		private void OnBattleEnded(BattleResult obj)
		{
			_battle.BattleEnded -= OnBattleEnded;
			GlobalEntities.DestroyDeadEntities();
			GlobalEntities.Clear();
			_playerTeam = new List<Entity>();
			if(obj.ResultType == BattleResult.BattleResultType.Win)
			{
				_playerTeam.AddRange(obj.Winner.GetListCopy().Where(e => !e.IsDead));
			}
			else
			{
				ResetPlayerData();
				_playerTeam.AddRange(_playerTeamInitial);
			}
			StartBattle();
		}
	}
}