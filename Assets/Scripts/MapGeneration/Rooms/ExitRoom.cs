using Content;
using DefaultNamespace.Content;
using Unity.Mathematics;
using UnityEngine;

namespace MapGeneration.Rooms
{
	public class ExitRoom : RoomTrigger
	{
		
		[SerializeField] private Exit _exitPrefab;

		private void Start()
		{
			Exit exit =Instantiate(_exitPrefab, MeshBulilder.I.ConvertPoint(Room.GetCenter()), quaternion.identity);
			exit.gameObject.transform.parent = transform;
		}
	}
}