using System;
using UnityEngine;

namespace Characters
{
	public class CharacterControlsProvider : MonoBehaviour
	{
		public event Action<Character> OnNewCharacter;
		public event Action<Character> OldCharacterReplaced;

		protected Character ControlledCharacter;
		protected CharacterMover CharacterMover;

		protected virtual void Awake()
		{
			Character character = GetComponentInParent<Character>();
			if (character == null)
			{
				throw new Exception("CharacterControlsProvider must be attached to a character");
			}
			
			ControlledCharacter = character;
			character.ControlsProvider = this;
			
			transform.parent = null;
		}

		protected virtual void Start()
		{
			SetCharacter(ControlledCharacter);
		}

		public void SetCharacter(Character newCharacter)
		{
			if (ControlledCharacter != null)
			{
				OldCharacterReplaced?.Invoke(ControlledCharacter);
			}
			
			ControlledCharacter = newCharacter;
			Debug.Log(ControlledCharacter + " controlled");
			CharacterMover = ControlledCharacter.CharacterMover;
			OnNewCharacter?.Invoke(ControlledCharacter);
		}

		private void OnCharacterDestroy()
		{
		}
	}
}