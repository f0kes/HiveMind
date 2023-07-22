using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Battle;
using DefaultNamespace;
using UnityEngine;

namespace GameState
{
	public class GameStateController : MonoBehaviour
	{
		[SerializeField] private List<Entity> _playerTeamInitial;
		[SerializeField] private List<Entity> _enemyTeamInitial;


		private List<Entity> _playerTeam = new List<Entity>();

		private Battle _battle;
		private void Awake()
		{
			_playerTeam.AddRange(_playerTeamInitial);
		}
		private void Start()
		{
			StartBattle();
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
				_playerTeam.AddRange(_playerTeamInitial);
			}
			StartBattle();
		}
	}
}