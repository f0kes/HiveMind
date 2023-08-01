using System;
using System.Collections.Generic;
using Characters;
using DefaultNamespace;

namespace Combat.Battle
{
	public interface IBattle
	{
		event Action<BattleResult> BattleEnded;
		IEntityRegistry EntityRegistry{get;}

		void StartBattle(IEnumerable<Character> playerTeam, IEnumerable<Character> enemyTeam);
	}
}