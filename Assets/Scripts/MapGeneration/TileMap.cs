namespace MapGeneration
{
	public class TileMap
	{
		public TileMap Root { get; private set; }

		private int _xPos;
		private int _zPos;

		private int _xSize;
		private int _zSize;

		private int[,] _map;


		public int XSize => _xSize;

		public int ZSize => _zSize;

		public int this[int x, int z]
		{
			get { return _map[x, z]; }
			set { _map[x, z] = value; }
		}


		public TileMap(TileMap from)
		{
			_xSize = from._xSize;
			_zSize = from._zSize;
			_map = new int[_xSize, _zSize];

			for (int x = 0; x < _xSize; x++)
			{
				for (int z = 0; z < _zSize; z++)
				{
					_map[x, z] = from._map[x, z];
				}
			}

			Root = this;
		}
		
		public TileMap(int xSize, int zSize, int xPos = 0, int zPos = 0)
		{
			_xSize = xSize;
			_zSize = zSize;
			var tilemap = new int[_xSize, _zSize];
			for (var x = 0; x < _xSize; x++)
			{
				for (var z = 0; z < _zSize; z++)
				{
					tilemap[x, z] = 0;
				}
			}

			_xPos = xPos;
			_zPos = zPos;
			_map = tilemap;
			Root = this;
		}

		public TileMap GetSlice(int xMin, int xMax, int zMin, int zMax)
		{
			if (xMax > _xSize)
			{
				xMax = _xSize;
			}

			if (zMax > _zSize)
			{
				zMax = _zSize;
			}


			var xSize = xMax - xMin;
			var zSize = zMax - zMin;
			var tilemap = new TileMap(xSize, zSize, xMin + _xPos, zMin + _zPos);
			for (var x = 0; x < xSize; x++)
			{
				for (var z = 0; z < zSize; z++)
				{
					tilemap[x, z] = _map[x + xMin, z + zMin];
				}
			}

			tilemap.Root = Root;
			return tilemap;
		}

		public int GetRootNeighbour(int x, int z, Direction direction)
		{
			x += _xPos;
			z += _zPos;
			return Root.GetNeighbour(x, z, direction);
		}

		private int GetNeighbour(int x, int z, Direction direction)
		{
			if (x < 0 || x >= _xSize || z < 0 || z >= _zSize)
				return -1;
			int neighbourX = x, neighbourZ = z;
			switch (direction)
			{
				case Direction.North:
					neighbourZ++;
					break;
				case Direction.East:
					neighbourX++;
					break;
				case Direction.South:
					neighbourZ--;
					break;
				case Direction.West:
					neighbourX--;
					break;
				default:
					return -1;
			}

			if (neighbourX < 0 || neighbourX >= _xSize || neighbourZ < 0 || neighbourZ >= _zSize)
				return -1;
			return _map[neighbourX, neighbourZ];
		}
	}

	public enum Direction : short
	{
		North = 0,
		East = 1,
		South = 2,
		West = 3,
		Up = 4,
		Down = 5
	}
}