using System;

namespace FireBase.Models
{
	[Serializable]
	public class HeroStatModel
	{
		public int Wins{get; set;}
		public int Losses{get; set;}
		public int PickCount{get; set;}
	}
}