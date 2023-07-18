using System;
using System.Collections.Generic;
using MapGeneration.Rooms;
using UnityEngine;

namespace Content
{
	public class ContentContainer : MonoBehaviour
	{
		[SerializeField] private List<Character.Character> _prisoners;
		[SerializeField] private List<MonsterSpawner> _monsterSpawners;
		[SerializeField] private MeshRenderer _sphereGizmo;

		public static ContentContainer I;

		private void Awake()
		{
			if (I == null)
			{
				I = this;
				transform.SetParent(null);
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}
		public Character.Character GetRandomPrisoner()
		{
			return _prisoners[UnityEngine.Random.Range(0, _prisoners.Count)];
		}
		public MonsterSpawner GetRandomMonsterSpawner()
		{
			return _monsterSpawners[UnityEngine.Random.Range(0, _monsterSpawners.Count)];
		}
		public MeshRenderer GetSphere()
		{
			return _sphereGizmo;
		}
	}
}