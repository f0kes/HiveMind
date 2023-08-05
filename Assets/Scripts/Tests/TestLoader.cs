using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using Cysharp.Threading.Tasks;
using GameState;
using UnityEngine;

namespace Tests
{
	public class TestLoader : MonoBehaviour
	{
		[SerializeField] private List<CharacterData> _playerCharacters;
		[SerializeField] private List<CharacterData> _enemyCharacters;
		[SerializeField] private int _level = 1;
		[SerializeField] private int _enemyLevel = 1;
		private async void Start()
		{
			await UniTask.Delay(500); //i don't know why this is necessary, but it is
			var playerCopy = _playerCharacters.Select(CharacterData.Copy).ToList();
			var enemyCopy = _enemyCharacters.Select(CharacterData.Copy).ToList();
			foreach(var character in playerCopy)
			{
				character.EntityData.Level = (uint)_level;
			}
			foreach(var character in enemyCopy)
			{
				character.EntityData.Level = (uint)_enemyLevel;
			}
			GameStateController.Instance.TryStartBattle(playerCopy, enemyCopy);
		}
	}
}