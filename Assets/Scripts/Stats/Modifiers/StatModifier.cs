using System;
using UnityEngine;

namespace Stats.Modifiers
{
	public abstract class StatModifier : ScriptableObject
	{
		public float Value;
		public float Priority;

		public string Name => GetType().ToString();
		private Func<float> _valueFunc;

		public void SetValFunc(Func<float> func)
		{
			_valueFunc = func;
		}

		public void ApplyMod(ref float finalStat, float baseValue)
		{
			if(_valueFunc != null)
			{
				Value = _valueFunc();
			}
			ApplyModChild(ref finalStat, baseValue);
		}
		protected abstract void ApplyModChild(ref float finalStat, float baseValue);

		public override string ToString()
		{
			return Name + ": " + Value;
		}
	}
}