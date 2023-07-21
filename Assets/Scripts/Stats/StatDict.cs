using System.Collections.Generic;
using Enums;
using Stats.Structures;

namespace Stats
{
	public class StatDict<TStatName> where TStatName : System.Enum
	{
		protected Stat[] _stats;

		public virtual float this[TStatName name, bool forceUpdate = false] => _stats[GetIndex(name)].GetValue(forceUpdate);
		public StatDict(float baseValue = 0, uint level = 1)
		{
			_stats = new Stat[System.Enum.GetValues(typeof(TStatName)).Length];
			foreach(TStatName name in System.Enum.GetValues(typeof(TStatName)))
			{
				var stat = new Stat(baseValue, level);
				_stats[(int)(object)name] = stat;
			}
		}
		public StatDict(StatDict<TStatName> other)
		{
			_stats = new Stat[System.Enum.GetValues(typeof(TStatName)).Length];
			foreach(TStatName name in System.Enum.GetValues(typeof(TStatName)))
			{
				var stat = new Stat(other[name]);
				_stats[(int)(object)name] = stat;
			}
		}
		public void SetLevel(uint level)
		{
			foreach(var stat in _stats)
			{
				stat.SetLevel(level);
			}
		}
		public Stat[] GetStats()
		{
			return _stats;
		}
		private int GetIndex(TStatName name)
		{
			return (int)(object)name;
		}
		public Stat GetStat(TStatName name)
		{
			return _stats[GetIndex(name)];
		}

		public void SetStat(TStatName name, Stat stat)
		{
			_stats[GetIndex(name)] = stat;
		}
		public StatDictFiltered<TStatName, T2> GetFiltered<T2>()
		{
			return new StatDictFiltered<TStatName, T2>(this);
		}

	}
}