﻿using System;
using System.Collections.Generic;
using MapGeneration.BSP;
using UnityEngine;

namespace MapGeneration.Rooms
{
	public class MonsterSpawner : Spawner
	{
		public bool IsCleared { get; private set; }

		public event Action OnClear;

		private RoomTrigger _room;

		[SerializeField] private List<Character> _monsters;
		private List<Character> _instantiatedMonsters = new List<Character>();
		private List<Character> _aliveMonsters = new List<Character>();

		public void SetRoom(RoomTrigger room)
		{
			_room = room;
		}

		protected override void Spawn()
		{
			foreach (var monster in _monsters)
			{
				//random position
				var offset = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f));
				Vector3 spawnPosition = transform.position + offset;
				var monsterInstance = Instantiate(monster, spawnPosition, Quaternion.identity);

				int tries = 0;
				bool breakLoop = false;

				while (!_room.Collider.bounds.Contains(monsterInstance.transform.position))
				{
					tries++;
					if (tries > 100)
					{
						Debug.LogError("Could not spawn monster");
						Destroy(monsterInstance);
						breakLoop = true;
						break;
					}

					monsterInstance.Teleport(spawnPosition, _room);
				}

				if (breakLoop) continue;

				monsterInstance.transform.parent = transform;
				_instantiatedMonsters.Add(monsterInstance);
				_aliveMonsters.Add(monsterInstance);
				monsterInstance.OnDeath += (e) => OnMonsterDeath(monsterInstance);
				monster.SetTeam(1);
			}

			if (_monsters.Count == 0)
				OnClear?.Invoke();
			else
				IsCleared = false;
		}

		private void OnMonsterDeath(Character monsterInstance)
		{
			if (_aliveMonsters.Contains(monsterInstance))
			{
				_aliveMonsters.Remove(monsterInstance);
				if (_aliveMonsters.Count == 0)
				{
					IsCleared = true;
					OnClear?.Invoke();
				}
			}
		}

		protected override void Despawn()
		{
			foreach (var monster in _aliveMonsters)
			{
				Destroy(monster.gameObject);
			}
		}

		private void OnDestroy()
		{
			foreach (var monster in _instantiatedMonsters)
			{
				Destroy(monster.ControlsProvider.gameObject);
			}
		}
	}
}