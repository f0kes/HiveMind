using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Combat.Spells;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace.Configs
{
	[CreateAssetMenu(fileName = "ContentDatabase", menuName = "Configs/ContentDatabase")]
	public class ContentDatabase : ScriptableObject
	{
		[FormerlySerializedAs("Characters")]
		[SerializeField] private List<CharacterData> _characters;
		[FormerlySerializedAs("Spells")]
		[SerializeField] private List<BaseSpell> _spells;

		private List<CharacterData> _charactersCopy;

		public List<CharacterData> Characters
		{
			get => _charactersCopy;
			set => _characters = value;
		}

		public List<BaseSpell> Spells
		{
			get => _spells;
			set => _spells = value;
		}

		public void Init()
		{
			_charactersCopy = _characters.Select(CharacterData.Copy).ToList();
		}
		public List<CharacterData> GenerateCharacterPool(int repeats, Func<CharacterData, bool> predicate = null)
		{
			var result = new List<CharacterData>();
			predicate ??= _ => true;
			var filteredCharacters = _charactersCopy.Where(characterData => predicate(characterData)).ToList();
			foreach(var characterData in filteredCharacters)
			{
				for(var i = 0; i < repeats; i++)
				{
					var entry = CharacterData.Copy(characterData);
					result.Add(entry);
				}
			}
			return result;
		}
		public static Func<CharacterData, bool> Purchasable => data =>
		{
			var result = true;
			result &= !data.EntityData.Tags.Contains(EntityTag.TokenCharacter);
			result &= !data.IsToken;
			return result;
		}; 
		
	}
}