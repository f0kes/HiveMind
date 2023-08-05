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
	public class Battle : IBattle
	{
		public event Action<BattleResult> BattleEnded;

		public IEntityRegistry EntityRegistry{get; private set;}

		private Dictionary<uint, List<SpawnPoint>> _spawnPoints = new();
		private List<Character> _toSpawn = new();

		private uint _teamsLength;
		private uint _teamsAlive;
		public Battle(IEntityRegistry entityRegistry)
		{
			EntityRegistry = entityRegistry;
			
		}
		public Battle()
		{
			var registry = new EntityRegistry();

			EntityRegistry = registry;
		}
		public void StartBattle(IEnumerable<Character> playerTeam, IEnumerable<Character> enemyTeam)
		{
			_toSpawn.AddRange(playerTeam);
			_toSpawn.AddRange(enemyTeam);

			Spawn();
			AssignCharacterToPlayer();

			EntityRegistry.OneRemainingTeam += OnOneRemainingTeam;
		}

		private void OnOneRemainingTeam(EntityTeam obj)
		{
			EntityRegistry.OneRemainingTeam -= OnOneRemainingTeam;
			var resultType = obj.IsPlayerTeam() ? BattleResult.BattleResultType.Win : BattleResult.BattleResultType.Lose;
			BattleEnded?.Invoke(new BattleResult { ResultType = resultType, Winner = obj });
			EntityRegistry.Clear();
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
		private void Spawn()
		{
			GetSpawnPointsOnScene();
			var count = _toSpawn.Count;
			var pointers = new int[_spawnPoints.Keys.Count];
			for(var i = 0; i < count; i++)
			{
				var character = _toSpawn[i];
				var team = character.Team;
				var spawnPoint = _spawnPoints[team][pointers[team]];
				//todo: something with add to character team
				pointers[team]++;

				character.transform.position = spawnPoint.transform.position;
				character.gameObject.SetActive(true);
			}
		}
		private void AssignCharacterToPlayer()
		{
			InputHandler.Instance.AssignRandomCharacter(0);
		}
	}
}