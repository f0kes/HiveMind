using System.Collections.Generic;
using MapGeneration.Rooms;
using UnityEngine;

namespace MapGeneration.BSP
{
	public class Room
	{
		private List<Room> _corridors = new List<Room>();
		public RoomType Type;
		protected int Left, Right, Top, Bottom;

		public int GetWidth()
		{
			return Right - Left + 1;
		}

		public int GetHeight()
		{
			return Top - Bottom + 1;
		}

		public void AddCorridor(Room corridor)
		{
			_corridors.Add(corridor);
		}
		public List<Room> GetCorridors()
		{
			return new List<Room>(_corridors);
		}

		public Room(int left, int right, int top, int bottom)
		{
			this.Left = left;
			this.Right = right;
			this.Top = top;
			this.Bottom = bottom;
		}

		public virtual void AddToMap(TileMap map, TileType tile)
		{
			for (int x = Left; x <= Right; x++)
			{
				for (int y = Bottom; y <= Top; y++)
				{
					map[x, y] = (int)tile;
				}
			}
		}

		public void Move(int x, int y)
		{
			Left += x;
			Right += x;
			Top += y;
			Bottom += y;
		}
		public Vector2Int GetCenter()
		{
			return new Vector2Int(Left + GetWidth() / 2, Bottom + GetHeight() / 2);
		}
		public Vector2Int GetRandomPoint()
		{
			var x = Random.Range(Left+1, Right-1);
			var y = Random.Range(Top-1, Bottom+1);
			return new Vector2Int(x, y);
		}
		public int GetLeft()
		{
			return Left;
		}

		public int GetRight()
		{
			return Right;
		}

		public int GetTop()
		{
			return Top;
		}

		public int GetBottom()
		{
			return Bottom;
		}
		
		public bool Contains(Vector2Int convertPoint)
		{
			return convertPoint.x >= Left && convertPoint.x <= Right && convertPoint.y >= Bottom && convertPoint.y <= Top;
		}
	}
}