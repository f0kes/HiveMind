using System;
using System.Collections.Generic;
using System.Linq;
using Content;
using UnityEngine;

namespace MapGeneration.Rooms
{
	public class BattleRoom : RoomTrigger
	{
		public event Action OnBattleStart;

		public event Action OnBattleEnd;
		//[SerializeField] private MonsterSpawner _monsterSpawner;

		private List<MonsterSpawner> _monsterSpawners = new List<MonsterSpawner>();
		private List<MonsterSpawner> _instantiatedMonsterSpawners = new List<MonsterSpawner>();

		[SerializeField] private int _maxSpawners = 100;

		public void SetSpawnerCount(int count)
		{
			if (count > _maxSpawners)
			{
				count = _maxSpawners;
			}


			for (int i = 0; i < count; i++)
			{
				var monsterSpawner = ContentContainer.I.GetRandomMonsterSpawner();
				_monsterSpawners.Add(monsterSpawner);
			}
		}

		private void Start()
		{
			OnPlayerEnter += OnPlayerEnterHandler;
			foreach (var monsterSpawner in _monsterSpawners)
			{
				var newMonsterSpawner = Instantiate(monsterSpawner, MeshBulilder.I.ConvertPoint(Room.GetRandomPoint()),
					Quaternion.identity, transform);
				newMonsterSpawner.SetRoom(this);
				_instantiatedMonsterSpawners.Add(newMonsterSpawner);
			}
		}

		private void OnPlayerEnterHandler()
		{
			foreach (var monsterSpawner in _instantiatedMonsterSpawners)
			{
				if (monsterSpawner.IsSpawned)
					return;
				monsterSpawner.StartSpawn();
				monsterSpawner.OnClear += OnSpawnerClear;
			}

			OnBattleStart?.Invoke();
		}

		private void OnSpawnerClear()
		{
			if (_instantiatedMonsterSpawners.Any(spawner => !spawner.IsCleared))
				return;

			OnBattleEnd?.Invoke();
		}

		
	}
}