using UnityEngine;

namespace MapGeneration.Rooms
{
	public class SpawnRoom : RoomTrigger
	{
		public Vector3 GetSpawnPosition()
		{
			return MeshBulilder.I.ConvertPoint(Room.GetRandomPoint());
		}
	}
}