using System.Collections.Generic;
using System.Linq;
using GameState;
using Stats.Modifiers;
using UnityEngine;

namespace Stats
{
	namespace Structures
	{
		[System.Serializable]
		public class Stat
		{
			[SerializeField] private float baseValue;

			public float BaseValue => baseValue;

			private float _lastValue;
			private int _lastUpdateTick = -1;
			private List<StatModifier> _modifiers;

			public List<StatModifier> Modifiers => _modifiers;
			

			public Stat()
			{
				_modifiers = new List<StatModifier>();
			}

			public Stat(float baseValue)
			{
				_modifiers = new List<StatModifier>();
				this.baseValue = baseValue;
			}
		

			public float GetValue(bool forceUpdate)
			{
				if(!forceUpdate && _lastUpdateTick >= Ticker.I.CurrentTick) return _lastValue;
				var result = baseValue;
				foreach(var mod in _modifiers)
				{
					mod.ApplyMod(ref result, baseValue);
				}
				_lastUpdateTick = Ticker.I.CurrentTick;
				_lastValue = result;
				return _lastValue;
			}

			public void SetBaseValue(float val)
			{
				baseValue = val;
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