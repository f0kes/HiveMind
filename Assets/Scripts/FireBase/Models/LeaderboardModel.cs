using System;
using System.Collections.Generic;
using System.Linq;

namespace FireBase.Models
{
	[Serializable]
	public class LeaderboardModel
	{
		public List<GameEntryModel> GameEntries{get; set;}
		public void Sort()
		{
			GameEntries.Sort((a, b) => b.LevelsBeaten.CompareTo(a.LevelsBeaten));
		}
		public int GetEntryPosition(GameEntryModel entry)
		{
			return GameEntries.Count(t => t.LevelsBeaten > entry.LevelsBeaten);
		}
	}
}