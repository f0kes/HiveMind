using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Combat.Battle.Zones;
using DefaultNamespace;
using Player;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Combat.Battle
{
	public class Battle
	{
		public event Action<BattleResult> BattleEnded;

		private EntityFactory _entityFactory;

		private Dictionary<BattleZone, List<Entity>> _battleZones = new();
		private Dictionary<Entity, BattleZone> _entityToZone = new();
		private Dictionary<int, EntityList> _teams = new();
		private List<Entity> _instantiatedEntities = new();

		private Dictionary<int, List<SpawnPoint>> _spawnPoints = new();

		private uint _teamsLength;
		private uint _teamsAlive;

		public void StartBattle(List<Entity> playerTeam, List<Entity> enemyTeam, EntityFactory factory, InputHandler playerInputHandler)
		{
			_entityFactory = factory;

			var playerEntityTeam = new EntityList(0);
			playerEntityTeam.SetList(playerTeam);

			var enemyEntityTeam = new EntityList(1);
			enemyEntityTeam.SetList(enemyTeam);

			AddTeam(new EntityList(0));
			AddTeam(new EntityList(1));

			Spawn(0, playerTeam);
			Spawn(1, enemyTeam);

			playerInputHandler.SetCharacter(playerTeam[0] as Character);
		}
		public void GetSpawnPointsOnScene()
		{
			var all = Object.FindObjectsOfType<SpawnPoint>();
			_spawnPoints = new Dictionary<int, List<SpawnPoint>>();
			foreach(var spawnPoint in all)
			{
				if(!_spawnPoints.ContainsKey(spawnPoint.TeamId))
					_spawnPoints.Add(spawnPoint.TeamId, new List<SpawnPoint>());
				_spawnPoints[spawnPoint.TeamId].Add(spawnPoint);
			}
		}
		public void ClearTeams()
		{
			if(_teams == null) return;
			foreach(var team in _teams.Values)
			{
				team.OnTeamWiped -= OnTeamWiped;
			}
			_teams.Clear();
			_teamsLength = 0;
			_teamsAlive = 0;
		}
		public void AddTeam(EntityList team)
		{
			_teams.Add(team.Id, team);
			_teamsLength++;
			_teamsAlive++;
		}

		private void OnTeamWiped(EntityList list)
		{
			Debug.Log("Team wiped");
			_teamsAlive--;

			if(_teamsAlive > 1) return;
			EndBattle();
		}
		private void EndBattle()
		{
			BattleResult.BattleResultType resultType;
			Debug.Log("Battle ended");
			var winner = _teams.Values.FirstOrDefault(t => t.Any());
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

		public void Spawn(int team, List<Entity> toSpawn)
		{
			GetSpawnPointsOnScene();

			var spawnPoints = _spawnPoints[team];
			var entities = toSpawn;

			for(var i = 0; i < Mathf.Min(entities.Count, spawnPoints.Count); i++)
			{
				var entityPrefab = entities[i];
				var spawnPoint = spawnPoints[i];

				var instance = _entityFactory.CreateEntity(entityPrefab);
				_teams[team].Add(instance);
				instance.SetTeam((ushort)team);
				_instantiatedEntities.Add(instance);
				var transform = spawnPoint.transform;
				SpawnEntity(instance, transform.position, transform.rotation);
				
			}
		}
		public void MoveEntityToZone(Entity entity, BattleZone zone)
		{
			if(_entityToZone.ContainsKey(entity))
			{
				var oldZone = _entityToZone[entity];
				_battleZones[oldZone].Remove(entity);
			}
			else
			{
				_entityToZone.Add(entity, zone);
			}
			if(!_battleZones.ContainsKey(zone))
				_battleZones.Add(zone, new List<Entity>());
			_battleZones[zone].Add(entity);
		}
		public Entity SpawnEntity(Entity entityInstance, Vector3 pos, Quaternion rot)
		{
			entityInstance.gameObject.SetActive(true);
			var transform = entityInstance.transform;
			transform.position = pos;
			transform.rotation = rot;
			entityInstance.Events.Death += OnEntityDeath;
			entityInstance.Events.Ressurect += OnEntityRessurect;

			MoveEntityToZone(entityInstance, BattleZone.Alive);
			return entityInstance;
		}

		private void OnEntityRessurect(Entity obj)
		{
			MoveEntityToZone(obj, BattleZone.Alive);
		}

		private void OnEntityDeath(Entity obj)
		{
			MoveEntityToZone(obj, BattleZone.Dead);
		}

		public List<Entity> GetEntitiesInZone(BattleZone zone)
		{
			return _battleZones[zone];
		}
		public List<Entity> GetEntitiesInRange(Vector3 position, float range)
		{
			var alive = _battleZones[BattleZone.Alive];
			return alive.Where(x => Vector3.Distance(x.transform.position, position) <= range).ToList();
		}

		public EntityList GetTeam(ushort team)
		{
			return _teams[team];
		}

		public List<Entity> GetAllEntities()
		{
			return _instantiatedEntities;
		}


	}
}