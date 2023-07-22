using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Combat.Battle
{
	public class Battle
	{
		public event Action<BattleResult> BattleEnded;

		private List<EntityList> _teams = new();
		private Dictionary<EntityList, List<SpawnPoint>> _spawnPoints = new();
		private Dictionary<Entity, Entity> _originalEntities = new();
		private uint _teamsLength;
		private uint _teamsAlive;

		public void StartBattle(List<Entity> playerTeam, List<Entity> enemyTeam)
		{
			var teams = new List<EntityList>();
			
			var playerEntityTeam = new EntityList(0);
			playerEntityTeam.SetList(playerTeam);

			var enemyEntityTeam = new EntityList(1);
			enemyEntityTeam.SetList(enemyTeam);

			teams.Add(new EntityList(0));
			teams.Add(new EntityList(1));

			SetTeams(teams);
			GetSpawnPointsOnScene();
			Spawn();
		}
		public void GetSpawnPointsOnScene()
		{
			var all = Object.FindObjectsOfType<SpawnPoint>();
			_spawnPoints = new Dictionary<EntityList, List<SpawnPoint>>();
			foreach(var spawnPoint in all)
			{
				if(!_spawnPoints.ContainsKey(_teams[spawnPoint.TeamId]))
					_spawnPoints.Add(_teams[spawnPoint.TeamId], new List<SpawnPoint>());
				_spawnPoints[_teams[spawnPoint.TeamId]].Add(spawnPoint);
			}
		}
		public void SetTeams(List<EntityList> teams)
		{
			if(_teams != null)
			{
				foreach(var team in _teams)
				{
					team.OnTeamWiped -= OnTeamWiped;
				}
			}
			_teams = teams;
			_teamsLength = (uint)teams.Count;
			_teamsAlive = _teamsLength;
			foreach(var team in teams)
			{
				team.OnTeamWiped += OnTeamWiped;
			}
		}

		private void OnTeamWiped(EntityList list)
		{
			_teamsAlive--;

			if(_teamsAlive > 1) return;
			EndBattle();
		}
		private void EndBattle()
		{
			BattleResult.BattleResultType resultType;
			var winner = _teams.FirstOrDefault(t => t.Any());
			if(winner == null)
			{
				resultType = BattleResult.BattleResultType.Draw;
			}
			else
			{
				resultType = winner.IsPlayerTeam() ? BattleResult.BattleResultType.Win : BattleResult.BattleResultType.Lose;
			}

			BattleEnded?.Invoke(new BattleResult { ResultType = resultType, Winner = winner });
		}

		public void Spawn()
		{
			GetSpawnPointsOnScene();
			foreach(var team in _teams)
			{
				var spawnPoints = _spawnPoints[team];
				var entities = team.ToList();
				for(var i = 0; i < Mathf.Min(entities.Count, spawnPoints.Count); i++)
				{
					var entity = entities[i];
					var spawnPoint = spawnPoints[i];
					var copy = Object.Instantiate(entity, spawnPoint.transform.position, Quaternion.identity);
					_originalEntities.Add(copy, entity);
				}
			}
		}


	}
}