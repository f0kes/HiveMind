using System;
using Enums;
using Stats;
using UnityEngine;

namespace DefaultNamespace
{
	[Serializable]
	public class EntityData
	{
		public Sprite Icon;
		public string Name;
		public EntityTag Tags;
		public SerializableCharacterStats Stats;
		public uint Level;
	}
}