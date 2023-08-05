using Combat.Battle;

namespace Combat
{
	public abstract class BattleSystem : IBattleSystem
	{
		protected IBattle Battle;
		protected bool Working;
		protected BattleSystem(IBattle battle)
		{
			Battle = battle;
		}

		public virtual void Start()
		{
			Working = true;
			SubscribeToEvents();
		}
		public virtual void Stop()
		{
			Working = false;
			UnsubscribeFromEvents();
		}

		public abstract void SubscribeToEvents();

		public abstract void UnsubscribeFromEvents();
	}
}