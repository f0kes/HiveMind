using System;
using Combat;
using DefaultNamespace;
using Enums;
using UnityEngine;

namespace Characters
{
	public class CharacterControlsProvider : MonoBehaviour
	{
		private bool _swapped;
		private Characters.Character _swapTarget;

		public event Action<Characters.Character> OnNewCharacter;
		public event Action<Characters.Character> OldCharacterReplaced;

		protected Characters.Character ControlledCharacter;
		protected CharacterMover CharacterMover;

		protected virtual void Awake()
		{
			Characters.Character character = GetComponentInParent<Characters.Character>();
			if(character == null)
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

		//TODO: Move to elsewhere
		public CastResult SwapWithNew(Character other, bool forceSwap = false)
		{
			if(other == ControlledCharacter) return new CastResult(CastResultType.Fail, "Can't swap with self");
			if(other.IsDead) return new CastResult(CastResultType.Fail, "Can't swap with dead character");
			var team = ControlledCharacter.GetTeam();
			if(!team.CanSwap() && !forceSwap) return new CastResult(CastResultType.Fail, "Swap on cooldown");
			ControlledCharacter.Events.SwappedWithCharacter?.Invoke(other);
			if(_swapped)
			{
				SwapBack();
			}

			_swapped = true;
			_swapTarget = ControlledCharacter;

			Swap(other);
			return new CastResult(CastResultType.Success);
		}

		public void SwapBack()
		{
			Debug.Log("SwapBack" + " " + _swapTarget);
			_swapped = false;
			Swap(_swapTarget);
			_swapTarget = null;
		}

		private void Swap(Characters.Character other)
		{
			var otherControlsProvider = other.ControlsProvider;

			otherControlsProvider.SetCharacter(ControlledCharacter);
			SetCharacter(other);
		}

		public void SetCharacter(Characters.Character newCharacter)
		{
			if(ControlledCharacter != null)
			{
				ControlledCharacter.Events.Death -= OnCharacterDeath;
				ControlledCharacter.Events.Ressurect -= OnCharacterRessurect;
				OldCharacterReplaced?.Invoke(ControlledCharacter);
			}

			ControlledCharacter = newCharacter;
			ControlledCharacter.ControlsProvider = this;
			ControlledCharacter.Events.Death += OnCharacterDeath;
			ControlledCharacter.Events.Ressurect += OnCharacterRessurect;
			CharacterMover = ControlledCharacter.CharacterMover;
			if(!ControlledCharacter.IsDead)
			{
				gameObject.SetActive(true);
			}
			OnNewCharacter?.Invoke(ControlledCharacter);
		}

		private void OnCharacterDeath(Entity obj)
		{
			gameObject.SetActive(false);
		}
		private void OnCharacterRessurect(Entity obj)
		{
			gameObject.SetActive(true);
		}

		private void OnCharacterDestroy()
		{
		}
	}
}