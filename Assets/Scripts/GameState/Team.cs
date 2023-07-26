namespace GameState
{
	public class Team
	{
		public TeamData Data{get; private set;}
		public Team(TeamData data)
		{
			Data = data;
		}

		public bool CanCast()
		{
			return Data.GetCastCooldown() <= 0f;
		}
	}
}