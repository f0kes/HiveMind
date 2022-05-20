using UnityEngine;

namespace MapGeneration.Rooms
{
	public abstract class Spawner : MonoBehaviour
	{
		public bool IsSpawned { get;private set; }

		public void StartSpawn()
		{
			if(IsSpawned) return;
			IsSpawned = true;
			Spawn();
		}
		public void StartDespawn()
		{
			if(!IsSpawned) return;
			IsSpawned = false;
			Despawn();
		}
		protected abstract void Spawn();
		protected abstract void Despawn();
	}
}