using Characters;
using Events;
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
			var characterTeam = character.GetTeam();
			foreach(var t in characterTeam)
			{
				var c = t as Character;
				if(c == null) continue;
				AddMana(c);
			}
		}
		private void AddMana(Character character)
		{
			character.SetMana(character.CurrentMana + _manaOnSwap);
		}
	}
}