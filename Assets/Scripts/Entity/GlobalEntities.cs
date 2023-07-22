using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
	public static class GlobalEntities
	{
		public static event Action<ushort, Entity> OnEntityAdded;


		private static List<Entity> _allEntities = new();
		private static List<Entity> _aliveEntities = new();
		private static Dictionary<ushort, EntityList> _teams = new();
		private static Dictionary<ushort, EntityList> _graveyards = new();

		static GlobalEntities()
		{
			//on scene loaded clear the list
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

		private static void OnSceneUnloaded(Scene x)
		{
			Clear();
		}
		public static void Clear()
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

		public static EntityList GetTeam(ushort team)
		{
			if(_teams.ContainsKey(team))
			{
				return _teams[team];
			}
			else
			{
				_teams.Add(team, new EntityList(team));
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
		public static void SetTeam(ushort team, EntityList list)
		{
			if(_teams.ContainsKey(team))
			{
				_teams[team] = list;
			}
			else
			{
				_teams.Add(team, list);
			}
		}

		public static List<Entity> GetEntitiesOnTeam(ushort team)
		{
			if(_teams.ContainsKey(team))
			{
				return new List<Entity>(_teams[team]);
			}
			else
			{
				_teams.Add(team, new EntityList(team));
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
				_graveyards.Add(team, new EntityList(team));
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
				var temp = _teams.ContainsKey(team);
				try
				{
					//here is the problem, when entitylist is instantied it adds itself to the team
					_teams.Add(team, new EntityList(team) { entity });
				}
				catch(Exception e)
				{
					Debug.LogError(team + " " + temp + " " + _teams.ContainsKey(team) + " " + e.Message + " " + e.StackTrace);
				}
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
				_graveyards.Add(entity.Team, new EntityList(entity.Team) { entity });
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
				_teams.Add(entity.Team, new EntityList(entity.Team) { entity });
			}

			entity.Events.Death += OnEntityDeath;
			entity.Events.Ressurect -= OnEntityRessurect;
		}

		public static void DestroyDeadEntities()
		{
			foreach(var entity in _graveyards.SelectMany(team => team.Value))
			{
				Object.Destroy(entity.gameObject);
			}
		}
	}
}