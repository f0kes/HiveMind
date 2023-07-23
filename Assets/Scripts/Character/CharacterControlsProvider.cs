using System;
using Combat;
using Combat.Battle;
using DefaultNamespace;
using Enums;
using UnityEngine;

namespace Characters
{
	public class CharacterControlsProvider : MonoBehaviour //
	{

		public event Action<Character> OnNewCharacter;
		public event Action<Character> OldCharacterReplaced;

		protected CharacterControlsProvider Replacing;
		protected Character ControlledCharacter;
		protected CharacterMover CharacterMover;

		protected virtual void Awake()
		{
			var character = GetComponentInParent<Character>();
			ControlledCharacter = character;
		}

		protected virtual void Start()
		{
			if(ControlledCharacter == null) return;
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

			Swap(other);
			return new CastResult(CastResultType.Success);
		}


		private void Swap(Character other)
		{
			if(Replacing != null)
			{
				Replacing.Enable();
			}
			var otherControlsProvider = other.ControlsProvider;
			Replacing = otherControlsProvider;
			Replacing.Disable();
			SetCharacter(other);
		}

		private void Disable()
		{
			enabled = false;
		}
		private void Enable()
		{
			enabled = true;
		}

		public void SetCharacter(Character newCharacter)
		{
			enabled = true;
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
			//Debug.Log(ControlledCharacter.CharacterMover);
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