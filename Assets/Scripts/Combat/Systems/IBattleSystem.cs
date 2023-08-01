namespace Combat
{
	public interface IBattleSystem
	{
		void Start();

		void Stop();

		void SubscribeToEvents();

		void UnsubscribeFromEvents();
	}
}