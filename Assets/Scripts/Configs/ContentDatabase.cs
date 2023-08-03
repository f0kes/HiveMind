using System.Collections.Generic;
using System.Linq;
using Characters;
using Combat.Spells;
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
		public List<CharacterData> GenerateCharacterPool(int repeats)
		{
			var result = new List<CharacterData>();
			foreach(var characterData in _characters)
			{
				for(var i = 0; i < repeats; i++)
				{
					var entry = CharacterData.Copy(characterData);
					result.Add(entry);
				}
			}
			return result;
		}
	}
}