using System;
using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace DefaultNamespace
{
	public interface IEntityRegistry
	{
		event Action<ushort, Entity> OnEntityAdded;
		event Action<EntityTeam> OneRemainingTeam;

		void Clear();

		List<Character> GetAllCharacters();

		List<Entity> GetAllEntitiesCopy();

		List<Entity> GetEntitiesInRange(Vector3 position, float range);

		List<Entity> GetEntitiesOnTeam(ushort team);

		List<Entity> GetGraveyard(ushort team);
		

		void RemoveEntityFromTeam(Entity entity);

		int GetAliveTeamsCount();

		EntityTeam GetTeam(ushort team);

		void SetTeam(ushort teamID, EntityTeam team);

		void AddToTeam(ushort team, Entity entity);
	}
}