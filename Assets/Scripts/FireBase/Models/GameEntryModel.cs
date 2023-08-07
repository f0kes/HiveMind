using System;
using System.Collections.Generic;
using System.Linq;
using Characters;

namespace FireBase.Models
{
	[Serializable]
	public class GameEntryModel
	{
		public string PlayerName{get; set;}
		public int LevelsBeaten{get; set;}
		public List<string> LastParty{get; set;}
		public Dictionary<string, int> HeroUseCount{get; set;}

		public void SetLastParty(IEnumerable<CharacterData> characterDatas)
		{
			LastParty = characterDatas.Select(x => x.EntityData.Name).ToList();
		}
		public void OnHeroesUse(IEnumerable<CharacterData> used)
		{
			HeroUseCount ??= new Dictionary<string, int>();
			foreach(var hero in used)
			{
				if(HeroUseCount.ContainsKey(hero.EntityData.Name))
				{
					HeroUseCount[hero.EntityData.Name]++;
				}
				else
				{
					HeroUseCount.Add(hero.EntityData.Name, 1);
				}
			}
		}
	}
}