using DefaultNamespace.Content;
using UnityEngine;

namespace MapGeneration.Rooms
{
	public class PrisonSpawner : Spawner
	{
		[SerializeField] private Prison _prisonPrefab;
		private Prison _spawnedPrison;
		protected  override void Spawn()
		{
			_spawnedPrison =Instantiate(_prisonPrefab, transform.position, Quaternion.identity);
		}

		protected override void Despawn()
		{
			if(_spawnedPrison != null)
				Destroy(_spawnedPrison.gameObject);
		}
	}
}