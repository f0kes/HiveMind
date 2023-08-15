using Characters;
using Combat.Battle;
using Events;
using Events.Implementations;
using UnityEngine;

namespace Combat
{
	public class SwapManaSystem : BattleSystem
	{
		private int _manaOnSwap;
		public SwapManaSystem(IBattle battle, int manaOnSwap) : base(battle) //TODO: list of characters, give mana to all of them
		{
			_manaOnSwap = manaOnSwap;
		}

		public override void SubscribeToEvents()
		{
			CharacterSwappedEvent.Subscribe(OnCharacterSwapped);
		}
		public override void UnsubscribeFromEvents()
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