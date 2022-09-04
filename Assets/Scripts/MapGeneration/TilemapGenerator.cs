using System.Collections.Generic;
using MapGeneration;
using MapGeneration.BSP;
using UnityEngine;

namespace DefaultNamespace.MapGeneration
{
	public class TilemapGenerator
	{
		private int _xSize;
		private int _zSize;
		public List<Room> Rooms { get; private set; } = new List<Room>();

		public TilemapGenerator(int xSize, int zSize)
		{
			_xSize = xSize;
			_zSize = zSize;
		}

		public TileMap GenerateTilemap()
		{
			var tilemap = new TileMap(_xSize, _zSize);
			for (var x = 0; x < _xSize; x++)
			{
				for (var y = 0; y < _zSize; y++)
				{
					tilemap[x, y] = 1;
				}
			}

			var leaves = new List<BinaryLeaf>();
			var root = new BinaryLeaf(1, _xSize - 1, _zSize - 1, 1);
			leaves.Add(root);
			bool didSplit = true;

			while (didSplit)
			{
				didSplit = false;
				for (int i = 0; i < leaves.Count; i++)
				{
					if (!leaves[i].Split())
						continue;

					leaves.Add(leaves[i].LeftLeaf);
					leaves.Add(leaves[i].RightLeaf);
					leaves.RemoveAt(i);
					didSplit = true;
				}
			}

			root.CreateRooms(20,40, 20, 40);
			Rooms = root.GetAllRooms();
			root.AddToMap(tilemap, TileType.Empty);


			return tilemap;
		}
	}
}