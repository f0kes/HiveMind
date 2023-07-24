using System.Collections.Generic;
using System.Linq;
using Characters;

namespace GameState
{
	public class CharacterFactory
	{
		private Dictionary<CharacterData, Character> _characters = new Dictionary<CharacterData, Character>();

		public List<Character> Create(IEnumerable<CharacterData> data, ushort teamId)
		{
			return data.Select(characterData => Create(characterData, teamId)).ToList();
		}

		public Character Create(CharacterData data, ushort teamId)
		{
			if(_characters.ContainsKey(data))
			{
				return _characters[data];
			}
			var character = Character.FromData(data);
			_characters.Add(data, character);
			character.SetTeam(teamId);
			return character;
		}
		public List<CharacterData> GetAliveOriginals()
		{
			return (from kvp in _characters where !kvp.Value.IsDead select kvp.Key).ToList();
		}
		public List<CharacterData> GetAliveOriginals(ushort teamId)
		{
			return (from kvp in _characters where !kvp.Value.IsDead && kvp.Value.Team == teamId select kvp.Key).ToList();
		}

	}
}