using System;
using UnityEngine;

namespace Characters
{
	public class CharacterControlsProvider : MonoBehaviour
	{
		private bool _swapped;
		private Character _swapTarget;
		
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
		
		public void SwapWithNew(Character other)
		{
			if (_swapped)
			{
				SwapBack();
			}
			_swapped = true;
			_swapTarget = other;

			Swap(other);
		}

		public void SwapBack()
		{
			_swapped = false;
			Swap(_swapTarget);
			_swapTarget = null;
		}

		private void Swap(Character other)
		{
			other.ControlsProvider.SetCharacter(ControlledCharacter);
			SetCharacter(other);
			(other.ControlsProvider, ControlledCharacter.ControlsProvider) =
				(ControlledCharacter.ControlsProvider, other.ControlsProvider);
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