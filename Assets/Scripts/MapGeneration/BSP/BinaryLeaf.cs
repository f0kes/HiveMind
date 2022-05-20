using System.Collections.Generic;
using UnityEngine;

namespace MapGeneration.BSP
{
	public class BinaryLeaf : Room
	{
		private static int _minWidth = 40;
		private static int _minHeight = 40;


		private static int _corridorWidth = 2;


		private bool _horizontalSplit;
		private bool _verticalSplit;

		public BinaryLeaf LeftLeaf;
		public BinaryLeaf RightLeaf;

		private Room _innerRoom;
		public List<Room> Corridors;

		public BinaryLeaf(int left, int right, int top, int bottom) : base(left, right, top, bottom)
		{
			_horizontalSplit = false;
			_verticalSplit = false;

			LeftLeaf = null;
			RightLeaf = null;

			if (GetHeight() < _minHeight || GetWidth() < _minWidth)
				Debug.LogError("Small room");
		}

		public bool IsLeaf()
		{
			return (_horizontalSplit == false) && (_verticalSplit == false);
		}

		public bool Split()
		{
			if (LeftLeaf != null || RightLeaf != null) return false;
			float rand = Random.value;
			bool splitH = (rand < 0.3f);

			var width = GetWidth();
			var height = GetHeight();
			if (width > height && width / height >= 1.25)
				splitH = false;
			else if (height > width && height / width >= 1.25)
				splitH = true;

			int max = (splitH ? GetHeight() : GetWidth()) - _minWidth;
			if (max < _minWidth) return false;
			int split = Random.Range(_minWidth, max);

			if (splitH)
			{
				HorizontalSplit(split);
				return true;
			}
			else
			{
				VerticalSplit(split);
				return true;
			}
		}

		private void HorizontalSplit(int split)
		{
			_horizontalSplit = true;


			LeftLeaf = new BinaryLeaf(GetLeft(), GetRight(), GetTop(), GetBottom() + split);
			RightLeaf = new BinaryLeaf(GetLeft(), GetRight(), GetBottom() + split, GetBottom());
		}

		private void VerticalSplit(int split)
		{
			_verticalSplit = true;


			LeftLeaf = new BinaryLeaf(GetLeft(), GetLeft() + split, GetTop(), GetBottom());
			RightLeaf = new BinaryLeaf(GetLeft() + split, GetRight(), GetTop(), GetBottom());
		}

		public void Trim(int trimSize)
		{
			Left += trimSize;
			Right -= trimSize;
			Top -= trimSize;
			Bottom += trimSize;

			LeftLeaf?.Trim(trimSize);
			RightLeaf?.Trim(trimSize);
		}

		public List<Room> GetAllRooms()
		{
			var rooms = new List<Room>();
			if (_innerRoom != null) rooms.Add(_innerRoom);
			var left = LeftLeaf?.GetAllRooms();
			var right = RightLeaf?.GetAllRooms();
			if (left != null) rooms.AddRange(left);
			if (right != null) rooms.AddRange(right);
			return rooms;
		}

		public Room GetInnerRoom()
		{
			if (_innerRoom != null) return _innerRoom;
			var left = LeftLeaf?.GetInnerRoom();
			var right = RightLeaf?.GetInnerRoom();
			switch (left)
			{
				case null when right == null:
					return null;
				case null:
					return right;
			}

			if (right == null)
				return left;
			var rand = Random.value;
			return rand < 0.5f ? left : right;
		}

		public void CreateCorridor(Room first, Room second, int width)
		{
			Corridors ??= new List<Room>();
			var start = first.GetRandomPoint();
			var end = second.GetRandomPoint();

			var corridorX = start.x < end.x
				? new Room(start.x, end.x, start.y + width, start.y - width)
				: new Room(end.x, start.x, start.y + width, start.y - width);

			var corridorY = start.y < end.y
				? new Room(start.x - width, start.x + width, end.y, start.y)
				: new Room(start.x - width, start.x + width, start.y, end.y);

			int corridoreYMove = end.x - start.x;
			corridorY.Move(corridoreYMove, 0);
			Corridors.Add(corridorX);
			Corridors.Add(corridorY);

			first.AddCorridor(corridorX);
			second.AddCorridor(corridorX);

			first.AddCorridor(corridorY);
			second.AddCorridor(corridorY);
		}


		public void CreateRooms(int minWidth, int maxWidth, int minHeight, int maxHeight)
		{
			if (IsLeaf())
			{
				var width = Random.Range(minWidth, maxWidth);
				var height = Random.Range(minHeight, maxHeight);
				var left = Random.Range(Left, Right - width);
				var bottom = Random.Range(Bottom, Top - height);
				//Debug.Log($"Creating room at {left}, {bottom} with size {width}x{height}");
				_innerRoom = new Room(left, left + width, bottom + height, bottom);
			}
			else
			{
				LeftLeaf?.CreateRooms(minWidth, maxWidth, minHeight, maxHeight);
				RightLeaf?.CreateRooms(minWidth, maxWidth, minHeight, maxHeight);

				if (LeftLeaf != null && RightLeaf != null)
				{
					CreateCorridor(LeftLeaf.GetInnerRoom(), RightLeaf.GetInnerRoom(), _corridorWidth);
				}
			}
		}

		public override void AddToMap(TileMap map, TileType tile)
		{
			if (IsLeaf())
			{
				_innerRoom.AddToMap(map, tile);
			}
			else
			{
				foreach (var corridor in Corridors)
				{
					corridor.AddToMap(map, TileType.Corridor);
				}

				LeftLeaf.AddToMap(map, tile);
				RightLeaf.AddToMap(map, tile);
			}
		}
	}
}