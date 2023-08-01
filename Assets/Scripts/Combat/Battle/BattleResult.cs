using DefaultNamespace;

namespace Combat.Battle
{
	public struct BattleResult
	{
		public enum BattleResultType
		{
			Win,
			Lose,
			Draw
		}
		public BattleResultType ResultType;
		public EntityTeam Winner;
	}
}