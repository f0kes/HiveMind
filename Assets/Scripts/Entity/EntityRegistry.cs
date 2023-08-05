using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace DefaultNamespace
{
	public class EntityRegistry : IEntityRegistry, ITeamRegistry
	{
		public event Action<EntityTeam> OneRemainingTeam;
		public event Action<ushort, Entity> OnEntityAdded;


		private List<Entity> _allEntities = new();
		private List<Character> _allCharacters = new();
		private List<Entity> _aliveEntities = new();
		private Dictionary<ushort, EntityTeam> _teams = new();
		private Dictionary<ushort, EntityTeam> _graveyards = new();

		public EntityRegistry()
		{
			//on scene loaded clear the list
			SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

		private void OnSceneUnloaded(Scene x)
		{
			Clear();
		}
		public void Clear()
		{
			_graveyards.Clear();
			_teams.Clear();
			_allEntities.Clear();
			_allCharacters.Clear();
		}
		public void RemoveEntityFromTeam(Entity entity)
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
		public int GetAliveTeamsCount()
		{
			return _teams.Values.Count(x => x.Count > 0);
		}

		public EntityTeam GetTeam(ushort team)
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
		public List<Character> GetAllCharacters()
		{
			return _allCharacters;
		}
		public List<Entity> GetAllEntitiesCopy()
		{
			return new List<Entity>(_allEntities);
		}
		public List<Entity> GetEntitiesInRange(Vector3 position, float range)
		{
			return GetAllEntitiesCopy().Where(x => Vector3.Distance(x.transform.position, position) <= range).ToList();
		}
		public void SetTeam(ushort teamID, EntityTeam team)
		{
			if(_teams.ContainsKey(teamID))
			{
				_teams[teamID] = team;
			}
			else
			{
				_teams.Add(teamID, team);
			}
		}

		public List<Entity> GetEntitiesOnTeam(ushort team)
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

		public List<Entity> GetGraveyard(ushort team)
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

		public void AddToTeam(ushort team, Entity entity)
		{
			if(!GetAllEntitiesCopy().Contains(entity))
			{
				OnEntityAdded?.Invoke(team, entity);
				_allEntities.Add(entity);
				if(entity is Character character)
				{
					_allCharacters.Add(character);
				}
			}
			if(_teams.ContainsKey(team))
			{
				_teams[team].Add(entity);
			}
			else
			{
				_teams.Add(team, new EntityTeam(team) { entity });
			}
			entity.Events.Death += OnEntityDeath;
		}

		private void OnEntityDeath(Entity entity)
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
			if(GetAliveTeamsCount() == 1)
			{
				OneRemainingTeam?.Invoke(_teams.Values.FirstOrDefault(x => x.Count > 0));
			}
			entity.Events.Death -= OnEntityDeath;
			entity.Events.Ressurect += OnEntityRessurect;
		}

		private void OnEntityRessurect(Entity entity)
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

		public void DestroyDeadEntities()
		{
			foreach(var entity in _graveyards.SelectMany(team => team.Value))
			{
				Object.Destroy(entity.gameObject);
			}
		}
	}
}