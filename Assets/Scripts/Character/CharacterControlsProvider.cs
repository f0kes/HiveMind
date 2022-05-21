using System;
using DefaultNamespace;
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
			_swapTarget = ControlledCharacter;

			Swap(other);
		}

		public void SwapBack()
		{
			Debug.Log("SwapBack" + " " + _swapTarget);
			_swapped = false;
			Swap(_swapTarget);
			_swapTarget = null;
		}

		private void Swap(Character other)
		{
			var otherControlsProvider = other.ControlsProvider;
			
			otherControlsProvider.SetCharacter(ControlledCharacter);
			SetCharacter(other);
		
		}

		public void SetCharacter(Character newCharacter)
		{
			if (ControlledCharacter != null)
			{
				ControlledCharacter.OnDeath -= OnCharacterDeath;
				OldCharacterReplaced?.Invoke(ControlledCharacter);
			}

			ControlledCharacter = newCharacter;
			ControlledCharacter.ControlsProvider = this;
			ControlledCharacter.OnDeath += OnCharacterDeath;
			Debug.Log(ControlledCharacter + " controlled");
			CharacterMover = ControlledCharacter.CharacterMover;
			OnNewCharacter?.Invoke(ControlledCharacter);
		}

		private void OnCharacterDeath(Entity obj)
		{
			gameObject.SetActive(false);
		}

		private void OnCharacterDestroy()
		{
		}
	}
}