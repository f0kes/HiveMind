using System;
using System.Collections.Generic;

namespace FireBase.Models
{
	[Serializable]
	public class PlayerModel
	{
		public Dictionary<string, GameEntryModel> GameEntries{get; set;}
	}
}