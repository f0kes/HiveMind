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

			character.ControlsProvider = this;
			SetCharacter(character);
			transform.parent = null;
		}

		public void SetCharacter(Character newCharacter)
		{
			if (ControlledCharacter != null)
			{
				OldCharacterReplaced?.Invoke(ControlledCharacter);
			}

			ControlledCharacter = newCharacter;
			CharacterMover = ControlledCharacter.CharacterMover;
			OnNewCharacter?.Invoke(ControlledCharacter);
		}

		private void OnCharacterDestroy()
		{
		}
	}
}