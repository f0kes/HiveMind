using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
	public static class EntityList
	{
		public static event Action<ushort, Entity> OnEntityAdded;


		private static List<Entity> _allEntities = new();
		private static List<Entity> _aliveEntities = new();
		private static Dictionary<ushort, EntityTeam> _teams = new();
		private static Dictionary<ushort, EntityTeam> _graveyards = new();

		static EntityList()
		{
			//on scene loaded clear the list
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

		private static void OnSceneUnloaded(Scene x)
		{
			_graveyards.Clear();
			_teams.Clear();
			_allEntities.Clear();
		}

		public static void RemoveEntityFromTeam(Entity entity)
		{
			if(_teams.ContainsKey(entity.Team) && _teams[entity.Team].Contains(entity))
			{
				_teams[entity.Team].Remove(entity);
			}


			if(_graveyards.ContainsKey(entity.Team) && _graveyards[entity.Team].Contains(entity))
			{
				_graveyards[entity.Team].Remove(entity);
			}
		}

		public static EntityTeam GetTeam(ushort team)
		{
			if(_teams.ContainsKey(team))
			{
				return _teams[team];
			}
			else
			{
				_teams.Add(team, new EntityTeam(team));
				return _teams[team];
			}
		}
		public static List<Entity> GetAllEntities()
		{
			return new List<Entity>(_allEntities);
		}
		public static List<Entity> GetEntitiesInRange(Vector3 position, float range)
		{
			return GetAllEntities().Where(x => Vector3.Distance(x.transform.position, position) <= range).ToList();
		}

		public static List<Entity> GetEntitiesOnTeam(ushort team)
		{
			if(_teams.ContainsKey(team))
			{
				return new List<Entity>(_teams[team]);
			}
			else
			{
				_teams.Add(team, new EntityTeam(team));
				return new List<Entity>(_teams[team]);
			}
		}

		public static List<Entity> GetGraveyard(ushort team)
		{
			if(_graveyards.ContainsKey(team))
			{
				return new List<Entity>(_graveyards[team]);
			}
			else
			{
				_graveyards.Add(team, new EntityTeam(team));
				return new List<Entity>(_graveyards[team]);
			}
		}

		public static void AddToTeam(ushort team, Entity entity)
		{
			if(!GetAllEntities().Contains(entity))
			{
				OnEntityAdded?.Invoke(team, entity);
				_allEntities.Add(entity);
			}


			if(_teams.ContainsKey(team))
			{
				_teams[team].Add(entity);
				entity.Events.Death += OnEntityDeath;
			}
			else
			{
				_teams.Add(team, new EntityTeam(team) { entity });
			}
		}

		private static void OnEntityDeath(Entity entity)
		{
			if(_teams.ContainsKey(entity.Team))
			{
				_teams[entity.Team].Remove(entity);
			}

			if(_graveyards.ContainsKey(entity.Team))
			{
				_graveyards[entity.Team].Add(entity);
			}
			else
			{
				_graveyards.Add(entity.Team, new EntityTeam(entity.Team) { entity });
			}

			entity.Events.Death -= OnEntityDeath;
			entity.Events.Ressurect += OnEntityRessurect;
		}

		private static void OnEntityRessurect(Entity entity)
		{
			if(_graveyards.ContainsKey(entity.Team))
			{
				_graveyards[entity.Team].Remove(entity);
			}

			if(_teams.ContainsKey(entity.Team))
			{
				_teams[entity.Team].Add(entity);
			}
			else
			{
				_teams.Add(entity.Team, new EntityTeam(entity.Team) { entity });
			}

			entity.Events.Death += OnEntityDeath;
			entity.Events.Ressurect -= OnEntityRessurect;
		}

	}
}