using System.Collections.Generic;
using Combat;
using Enums;
using UnityEngine;

namespace Characters
{
	public class CharacterSwapper
	{
		private Dictionary<Character, CharacterControlsProvider> _characterControlsProviders = new();
		private Dictionary<CharacterControlsProvider, bool> _swapStates = new();
		private Dictionary<CharacterControlsProvider, Character> _swapTargets = new();

		public void AddCharacter(Character character, CharacterControlsProvider characterControlsProvider)
		{
			_characterControlsProviders.Add(character, characterControlsProvider);
			_swapStates.Add(characterControlsProvider, false);
			_swapTargets.Add(characterControlsProvider, null);
		}
		public CastResult SwapWithNew(Character swapper, Character other, bool forceSwap = false)
		{
			var swapperControlsProvider = _characterControlsProviders[swapper];
			var otherControlsProvider = _characterControlsProviders[other];

			if(other == swapper) return new CastResult(CastResultType.Fail, "Can't swap with self");
			if(other.IsDead) return new CastResult(CastResultType.Fail, "Can't swap with dead character");
			var team = swapper.GetTeam();
			if(!team.CanSwap() && !forceSwap) return new CastResult(CastResultType.Fail, "Swap on cooldown");
			swapper.Events.SwappedWithCharacter?.Invoke(other);
			if(_swapStates[swapperControlsProvider])
			{
				SwapBack(swapper);
			}

			_swapStates[swapperControlsProvider] = true;
			_swapTargets[swapperControlsProvider] = swapper;

			Swap(swapper, other);
			return new CastResult(CastResultType.Success);
		}

		public void SwapBack(Character swapper)
		{
			var swapperCp = _characterControlsProviders[swapper];
			var other = _swapTargets[swapperCp];
			Debug.Log("SwapBack" + " " + other);
			_swapStates[swapperCp] = false;
			Swap(swapper, other);
			_swapTargets[swapperCp] = null;
		}

		private void Swap(Character swapper, Character other)
		{
			var swapperControlsProvider = _characterControlsProviders[swapper];
			var otherControlsProvider = _characterControlsProviders[other];

			otherControlsProvider.SetCharacter(swapper);
			swapperControlsProvider.SetCharacter(other);
		}
	}
}