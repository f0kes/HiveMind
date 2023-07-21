using System;
using DefaultNamespace.Settings;
using UnityEngine;

namespace Stats
{
	[Serializable]
	public class MinMaxStatRange
	{
		[SerializeField] private float _min;
		[SerializeField] private float _max;

		public float Min => _min;
		public float Max => _max;

		public uint Level{get; private set;}
		public float Value => _min + (_max - _min) * Level / GameSettings.MaxStatValue;
		public MinMaxStatRange()
		{
		}
		public MinMaxStatRange(float min, float max)
		{
			_min = min;
			_max = max;
		}
		public void SetLevel(uint level)
		{
			Level = level;
		}

	}
}