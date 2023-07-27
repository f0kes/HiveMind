using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using GameState;
using UnityEngine;

namespace Tests
{
	public class TestLoader : MonoBehaviour
	{
		[SerializeField] private List<CharacterData> _playerCharacters;
		[SerializeField] private List<CharacterData> _enemyCharacters;
		[SerializeField] private int _level = 1;
		private void Start()
		{
			var playerCopy = _playerCharacters.Select(CharacterData.Copy).ToList();
			var enemyCopy = _enemyCharacters.Select(CharacterData.Copy).ToList();
			foreach(var character in playerCopy)
			{
				character.EntityData.Level = (uint)_level;
			}
			foreach(var character in enemyCopy)
			{
				character.EntityData.Level = (uint)_level;
			}
			GameStateController.Instance.TryStartBattle(playerCopy, enemyCopy);
		}
	}
}