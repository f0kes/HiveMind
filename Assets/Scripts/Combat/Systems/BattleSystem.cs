using Combat.Battle;

namespace Combat
{
	public abstract class BattleSystem : IBattleSystem
	{
		protected IBattle Battle;
		protected BattleSystem(IBattle battle)
		{
			Battle = battle;
		}

		public abstract void Start();
		public abstract void Stop();

		public abstract void SubscribeToEvents();

		public abstract void UnsubscribeFromEvents();
	}
}