using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace FightGeneration
{
	public class FightGenerator
	{
		private List<CharacterData> _pool;
		private int _enemyCount;
		private uint _enemyLevel;

		public FightGenerator(List<CharacterData> pool, int enemyCount = 3, uint enemyLevel = 1)
		{
			_pool = pool;
			_enemyCount = enemyCount;
			_enemyLevel = enemyLevel;
		}

		public List<CharacterData> Generate()
		{
			var result = new List<CharacterData>();
			for(int i = 0; i < _enemyCount; i++)
			{
				var enemy = _pool[Random.Range(0, _pool.Count)];
				enemy = CharacterData.Copy(enemy);
				enemy.EntityData.Level = _enemyLevel;
				result.Add(enemy);
			}
			return result;
		}
	}
}