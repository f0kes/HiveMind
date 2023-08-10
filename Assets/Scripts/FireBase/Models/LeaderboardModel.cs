using System;
using System.Collections.Generic;
using System.Linq;

namespace FireBase.Models
{
	[Serializable]
	public class LeaderboardModel
	{
		public List<GameEntryModel> GameEntries{get; set;}
		public LeaderboardModel()
		{
			GameEntries = new List<GameEntryModel>();
		}
		public void Sort()
		{
			GameEntries.Sort((a, b) => b.LevelsBeaten.CompareTo(a.LevelsBeaten));
		}
		public int GetEntryPosition(GameEntryModel entry)
		{
			return GameEntries.Count(t => t.LevelsBeaten > entry.LevelsBeaten);
		}

		public void AddEntry(string playerName, GameEntryModel model)
		{
			var entry = GameEntries.FirstOrDefault(x => x.PlayerName == playerName);
			if(entry != null && entry.LevelsBeaten < model.LevelsBeaten)
			{
				GameEntries.Remove(entry);
				GameEntries.Add(model);
			}
			else if(entry == null)
			{
				GameEntries.Add(model);
			}
		}
	}
}