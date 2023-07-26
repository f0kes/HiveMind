using DefaultNamespace.Settings;
using Stats.Structures;

namespace GameState
{
	public class TeamData
	{
		private ushort _teamId;
		public ushort TeamId => _teamId;

		private Stat _swapCooldown;
		public Stat SwapCooldown => _swapCooldown;

		private Stat _castCooldown;
		public Stat CastCooldown => _castCooldown;
		


		private bool _subscribedToTicker;
		private float _swapCooldownTimer;
		private float _castCooldownTimer;

		public TeamData(ushort teamId, Stat swapCooldown, Stat castCooldown, bool subscribeToTicker = true)
		{
			_teamId = teamId;
			_swapCooldown = swapCooldown;
			_castCooldown = castCooldown;
			if(subscribeToTicker)
			{
				SubscribeToTicker();
			}
		}
		~TeamData()
		{
			if(_subscribedToTicker)
			{
				UnsubscribeFromTicker();
			}
		}
		public void SubscribeToTicker()
		{
			_subscribedToTicker = true;
			Ticker.OnTick += OnTick;
		}
		public void UnsubscribeFromTicker()
		{
			_subscribedToTicker = false;
			Ticker.OnTick -= OnTick;
		}

		private void OnTick(Ticker.OnTickEventArgs obj)
		{
			_swapCooldownTimer += Ticker.TickInterval;
			_castCooldownTimer += Ticker.TickInterval;
		}
		public void OnSwap()
		{
			_swapCooldownTimer = 0;
		}
		public bool IsPlayerTeam()
		{
			return _teamId == 0;
		}
		public void OnCast()
		{
			_castCooldownTimer = 0;
		}
		public float GetSwapCooldown()
		{
			return SwapCooldown - _swapCooldownTimer;
		}

		public float GetCastCooldown()
		{
			return CastCooldown - _castCooldownTimer;
		}
	}
}