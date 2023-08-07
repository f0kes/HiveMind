using System;
using System.Collections.Generic;

namespace FireBase.Models
{
	[Serializable]
	public class AllModel
	{
		public List<PlayerModel> PlayerModels{get; set;}
		public Dictionary<string, HeroStatModel> HeroStatModels{get; set;}
	}
}