using System;
using System.Collections.Generic;
using Combat.Spells;
using DefaultNamespace;
using Enums;
using Misc;
using UnityEngine;

namespace Characters
{
	[CreateAssetMenu(fileName = "CharacterData", menuName = "Characters/CharacterData")]
	public class CharacterData : ScriptableObject
	{
		public Character Prefab;
		public EntityData EntityData;
		public List<BaseSpell> Spells;
		public float AIDesirability = 1;
		public float AIThreat = 1;
		public int MaxMana = 10;
		public CharacterClass Class;
		public bool IsToken = false;

		private int _currentUseCooldown = 0;
		private int _nextUseCooldown = 1;
		private int _maxUseCooldown = 5;

		public int CurrentUseCooldown => _currentUseCooldown;
		public static CharacterData Copy(CharacterData original)
		{
			var result = CreateInstance<CharacterData>();
			result.Prefab = original.Prefab;
			result.EntityData = new EntityData(original.EntityData);
			result.Spells = new List<BaseSpell>(original.Spells);
			result.AIDesirability = original.AIDesirability;
			result.AIThreat = original.AIThreat;
			result.MaxMana = original.MaxMana;
			result.Class = original.Class;
			result._currentUseCooldown = original._currentUseCooldown;
			result._nextUseCooldown = original._nextUseCooldown;
			result._maxUseCooldown = original._maxUseCooldown;
			return result;
		}
		public TaskResult LaunchCooldown()
		{
			if(!CanUse()) return TaskResult.Failure("Can't use yet");
			_currentUseCooldown = _nextUseCooldown;
			_nextUseCooldown++;
			if(_nextUseCooldown > _maxUseCooldown)
				_nextUseCooldown = _maxUseCooldown;
			return TaskResult.Success;
		}
		public void DecrementCooldown()
		{
			_currentUseCooldown--;
			if(_currentUseCooldown < 0)
				_currentUseCooldown = 0;
		}
		public bool CanUse()
		{
			return _currentUseCooldown <= 0;
		}
		public void SetLevel(uint level)
		{
			EntityData.SetLevel(level);
		}
	}
}