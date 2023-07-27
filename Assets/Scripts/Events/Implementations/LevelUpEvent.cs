using DefaultNamespace;

namespace Events.Implementations
{
	public struct LevelUpData
	{
		public readonly Entity Character;
		public readonly int LevelIncreaseAmount;
		public LevelUpData(Entity character, int levelIncreaseAmount)
		{
			Character = character;
			LevelIncreaseAmount = levelIncreaseAmount;
		}
	}
	public class LevelUpEvent : GameEvent<LevelUpData>
	{

	}

}