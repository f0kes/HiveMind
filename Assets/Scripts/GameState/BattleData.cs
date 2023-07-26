using System.Collections.Generic;
using DefaultNamespace;

namespace GameState
{
	public class BattleData
	{
		private List<Entity> _entities;
		private List<Team> _teams;

		public List<Entity> Entities => _entities;
		public List<Team> Teams => _teams;

		public BattleData(List<Entity> entities, List<Team> teams)
		{
			_entities = entities;
			_teams = teams;
		}
	}
}