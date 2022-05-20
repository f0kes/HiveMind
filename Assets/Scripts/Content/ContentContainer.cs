using System;
using System.Collections.Generic;
using MapGeneration.Rooms;
using UnityEngine;

namespace Content
{
	public class ContentContainer : MonoBehaviour
	{
		[SerializeField] private List<Character> _prisoners;
		[SerializeField] private List<MonsterSpawner> _monsterSpawners;

		public static ContentContainer Instance;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}
		public Character GetRandomPrisoner()
		{
			return _prisoners[UnityEngine.Random.Range(0, _prisoners.Count)];
		}
		public MonsterSpawner GetRandomMonsterSpawner()
		{
			return _monsterSpawners[UnityEngine.Random.Range(0, _monsterSpawners.Count)];
		}
	}
}