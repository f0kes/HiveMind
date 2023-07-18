using System.Collections.Generic;
using JetBrains.Annotations;
using Stats.Modifiers;
using Stats.Structures;

namespace Stats
{

	public class StatDictFiltered<TStatName, TTag> : StatDict<TStatName> where TStatName : System.Enum
	{
		public struct Filter
		{
			public TStatName Name;
			public TTag Tag;
		}
		private HashSet<Filter> _existingFilters = new HashSet<Filter>();
		private HashSet<TTag> _persistentTags = new HashSet<TTag>();
		public Dictionary<Filter, List<StatModifier>> Modifiers{get; private set;}

		public StatDictFiltered(StatDict<TStatName> statDict, TTag[] tags)
		{
			_stats = statDict.GetStats();
			Modifiers = new Dictionary<Filter, List<StatModifier>>();
			foreach(var tag in tags)
			{
				_persistentTags.Add(tag);
			}
		}
		public StatDictFiltered(StatDict<TStatName> statDict)
		{
			_stats = statDict.GetStats();
			Modifiers = new Dictionary<Filter, List<StatModifier>>();
		}
		public StatDictFiltered<TStatName, TTag> WithFilter(TTag[] tags)
		{
			return new StatDictFiltered<TStatName, TTag>(this, tags);
		}

		public float this[TStatName name, TTag tag, bool forceUpdate = false]
		{
			get
			{
				var filter = new Filter { Name = name, Tag = tag };
				var baseValue = base[name, forceUpdate];
				if(!Modifiers.ContainsKey(filter)) return baseValue;
				foreach(var modifier in Modifiers[filter])
				{
					modifier.ApplyMod(ref baseValue, _stats[(int)(object)name].BaseValue);
				}
				return baseValue;
			}
		}

		public override float this[TStatName name, bool forceUpdate = false]
		{
			get
			{
				var baseValue = base[name, forceUpdate];
				foreach(var tag in _persistentTags)
				{
					var filter = new Filter { Name = name, Tag = tag };
					if(!Modifiers.ContainsKey(filter)) continue;
					foreach(var modifier in Modifiers[filter])
					{
						modifier.ApplyMod(ref baseValue, _stats[(int)(object)name].BaseValue);
					}
				}
				return baseValue;
			}
		}



		public void AddModifier(TStatName name, TTag tag, StatModifier modifier)
		{
			var filter = new Filter { Name = name, Tag = tag };

			if(_existingFilters.Contains(filter))
			{
				Modifiers[filter].Add(modifier);
			}
			else
			{
				_existingFilters.Add(filter);
				Modifiers.Add(filter, new List<StatModifier> { modifier });
			}
		}

	}
}