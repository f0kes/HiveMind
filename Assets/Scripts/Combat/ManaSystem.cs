using Characters;
using Events.Implementations;
using UnityEngine;

namespace Combat
{
	public class ManaSystem : ICombatSystem
	{
		private int _manaOnSwap;
		public ManaSystem(int manaOnSwap) //TODO: list of characters, give mana to all of them
		{
			_manaOnSwap = manaOnSwap;
		}

		public void Start()
		{
			SubscribeToEvents();
		}

		public void Stop()
		{
			UnsubscribeFromEvents();
		}

		public void SubscribeToEvents()
		{
			CharacterSwappedEvent.Subscribe(OnCharacterSwapped);
		}
		public void UnsubscribeFromEvents()
		{
			CharacterSwappedEvent.Unsubscribe(OnCharacterSwapped);
		}
		private void OnCharacterSwapped(CharacterSwappedData obj)
		{
			var character = obj.NewCharacter as Character;
			if(character == null) return;
			character.SetMana(character.CurrentMana + _manaOnSwap);
			Debug.Log("Mana on swap: " + _manaOnSwap);
		}
	}
}