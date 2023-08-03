using System;
using System.Collections.Generic;
using Characters;

namespace GameState
{
	public interface ICharacterFactory
	{
		List<Character> Create(IEnumerable<CharacterData> data, ushort teamId);

		Character Create(CharacterData data, ushort teamId);

		List<CharacterData> QueryOriginals(Func<Character, bool> predicate);
	}
}