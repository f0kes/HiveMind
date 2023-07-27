namespace Combat
{
	public interface ICombatSystem
	{
		void Start();

		void Stop();

		void SubscribeToEvents();

		void UnsubscribeFromEvents();
	}
}