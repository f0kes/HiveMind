using System;
using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace GameState
{
	public interface ICharacterFactory
	{
		List<Character> Create(IEnumerable<CharacterData> data, ushort teamId);

		Character Create(CharacterData data, ushort teamId);
		Character CreateInstant(CharacterData data, ushort teamId, Vector3 position);

		List<CharacterData> QueryOriginals(Func<Character, bool> predicate);
	}
}