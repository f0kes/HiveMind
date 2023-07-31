using Characters;
using UnityEngine;

namespace Player
{
	public struct Inputs
	{
		public struct MouseData
		{
			public bool Found;
			public Vector3 Position;
		}
		public struct MouseOverCharacterData
		{
			public bool Found;
			public Character Character;
		}
		public MouseData Mouse;
		public MouseOverCharacterData MouseOverCharacter;
		public Vector2 Move;
		public bool Cast;
		public bool Shoot;
		public bool Swap;
		public bool Cheats;
		public void Flush()
		{
			Mouse = new MouseData();
			MouseOverCharacter = new MouseOverCharacterData();
			Move = new Vector2();
			Cast = false;
			Shoot = false;
			Swap = false;
			Cheats = false;
		}
	}
}