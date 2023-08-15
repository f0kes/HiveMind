using Combat.Battle;
using GameState;

namespace Combat
{
	public class TimeManaSystem : BattleSystem
	{
		private int _manaPerTick;
		private float _tickTime;

		private float _timeSinceLastTick;
		public TimeManaSystem(IBattle battle, int manaPerTick, float tickTime) : base(battle)
		{
			_manaPerTick = manaPerTick;
			_tickTime = tickTime;
		}

		public override void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
		}

		public override void UnsubscribeFromEvents()
		{
			Ticker.OnTick -= OnTick;
		}
		private void OnTick(Ticker.OnTickEventArgs obj)
		{
			_timeSinceLastTick += Ticker.TickInterval;
			if(_timeSinceLastTick < _tickTime) return;
			_timeSinceLastTick = 0;
			foreach(var character in Battle.EntityRegistry.GetAllCharacters())
			{
				character.SetMana(character.CurrentMana + _manaPerTick);
			}
		}
	}
}