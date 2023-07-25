using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using DefaultNamespace;
using Player;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Combat.Battle
{
	public class Battle
	{
		public event Action<BattleResult> BattleEnded;

		private Dictionary<uint, List<SpawnPoint>> _spawnPoints = new();
		private List<Character> _toSpawn = new();

		private uint _teamsLength;
		private uint _teamsAlive;

		public void StartBattle(IEnumerable<Character> playerTeam, IEnumerable<Character> enemyTeam)
		{
			_toSpawn.AddRange(playerTeam);
			_toSpawn.AddRange(enemyTeam);

			Spawn();
			AssignCharacterToPlayer();

			GlobalEntities.OneRemainingTeam += OnOneRemainingTeam;
		}

		private void OnOneRemainingTeam(EntityList obj)
		{
			var resultType = obj.IsPlayerTeam() ? BattleResult.BattleResultType.Win : BattleResult.BattleResultType.Lose;
			BattleEnded?.Invoke(new BattleResult { ResultType = resultType, Winner = obj });
		}

		public void GetSpawnPointsOnScene()
		{
			var all = Object.FindObjectsOfType<SpawnPoint>();
			_spawnPoints = new Dictionary<uint, List<SpawnPoint>>();
			foreach(var spawnPoint in all)
			{
				var teamId = (uint)spawnPoint.TeamId;
				if(!_spawnPoints.ContainsKey(teamId))
				{
					_spawnPoints.Add(teamId, new List<SpawnPoint>());
				}
				_spawnPoints[teamId].Add(spawnPoint);
			}
		}
		public void Spawn()
		{
			GetSpawnPointsOnScene();
			var count = Mathf.Min(_spawnPoints.Count, _toSpawn.Count);
			var pointers = new int[_spawnPoints.Keys.Count];
			for(var i = 0; i < count; i++)
			{
				var character = _toSpawn[i];
				var team = character.Team;
				var spawnPoint = _spawnPoints[team][pointers[team]];
				pointers[team]++;

				character.transform.position = spawnPoint.transform.position;
			}
		}
		private void AssignCharacterToPlayer()
		{
			InputHandler.Instance.AssignRandomCharacter(0);
		}


	}
}