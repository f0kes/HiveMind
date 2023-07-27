using System.Collections.Generic;
using System.Linq;
using Combat.Spells;
using DefaultNamespace.Settings;
using GameState;
using Stats.Modifiers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Stats
{
	namespace Structures
	{
		[System.Serializable]
		public class Stat
		{
			[FormerlySerializedAs("baseValue")]
			[SerializeField] private float _baseValue;
			[FormerlySerializedAs("_valuePerLevel")]
			[SerializeField] private MinMaxStatRange _scalingRange = new MinMaxStatRange(1, GameSettings.MaxStatValue);

			public float BaseValue => _baseValue;

			private float _lastValue;
			private List<StatModifier> _modifiers;

			public List<StatModifier> Modifiers => _modifiers;


			public Stat()
			{
				_modifiers = new List<StatModifier>();
			}
			public Stat(Stat other)
			{
				_modifiers = new List<StatModifier>(other._modifiers);
				_baseValue = other._baseValue;
				_scalingRange = other._scalingRange;
			}

			public Stat(float baseValue, uint level = 1)
			{
				_modifiers = new List<StatModifier>();
				_baseValue = baseValue;
				SetLevel(level);
			}

			public void SetLevel(uint level)
			{
				_scalingRange.SetLevel(level);
				_baseValue = _scalingRange.Value;
			}
			public void SetScaling(MinMaxStatRange valuePerLevel)
			{
				_scalingRange = valuePerLevel;
			}


			public float GetValue(bool forceUpdate)
			{
				var result = _baseValue;
				foreach(var mod in _modifiers)
				{
					mod.ApplyMod(ref result, _baseValue);
				}
				_lastValue = result;
				return _lastValue;
			}

			public void SetBaseValue(float val)
			{
				_baseValue = val;
			}

			public float GetLastValue()
			{
				return _lastValue;
			}



			public void AddMod(StatModifier mod)
			{
				_modifiers.Add(mod);
				_modifiers = _modifiers.OrderBy(m => m.Priority).ToList();
			}


			public void RemoveMod(StatModifier mod)
			{
				if(!_modifiers.Contains(mod)) return;
				_modifiers.Remove(mod);
				_modifiers = _modifiers.OrderBy(m => m.Priority).ToList();
			}
			public static implicit operator float(Stat stat)
			{
				return stat.GetValue(true);
			}

			#region operators

			public static Stat operator +(Stat a, Stat b)
			{
				return new Stat(a.GetValue(true) + b.GetValue(true));
			}
			public static Stat operator -(Stat a, Stat b)
			{
				return new Stat(a.GetValue(true) - b.GetValue(true));
			}
			public static Stat operator *(Stat a, Stat b)
			{
				return new Stat(a.GetValue(true) * b.GetValue(true));
			}
			public static Stat operator /(Stat a, Stat b)
			{
				return new Stat(a.GetValue(true) / b.GetValue(true));
			}

			#endregion

		}
	}
}