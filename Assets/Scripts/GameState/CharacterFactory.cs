using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using UnityEngine;

namespace GameState
{
	public class CharacterFactory : ICharacterFactory
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
			character.gameObject.SetActive(false);
			return character;
		}

		public Character CreateInstant(CharacterData data, ushort teamId, Vector3 position)
		{
			var character = Create(data, teamId);
			character.transform.position = position;
			character.gameObject.SetActive(true);
			return character;
		}

		public List<CharacterData> QueryOriginals(Func<Character, bool> predicate)
		{
			return (from kvp in _characters where predicate(kvp.Value) select kvp.Key).ToList();
		}
	}
}