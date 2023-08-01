using System.Collections.Generic;
using System.Linq;
using Characters;
using Combat.Battle;
using Events.Implementations;
using GameState;
using UI;

namespace Combat
{
	public class FatigueSystem : BattleSystem
	{
		private float _timeToStartFatigue;
		private float _fatigueTickTime;
		private int _startFatigueValue;
		private int _fatigueIncrement;

		private float _timeSinceStart;
		private float _timeSinceLastFatigueTick;
		private int _currentFatigueValue;

		private bool _working;

		public FatigueSystem(IBattle battle, float timeToStartFatigue, float fatigueTickTime, int startFatigueValue, int fatigueIncrement) : base(battle)
		{
			_timeToStartFatigue = timeToStartFatigue;
			_fatigueTickTime = fatigueTickTime;
			_startFatigueValue = startFatigueValue;
			_fatigueIncrement = fatigueIncrement;
			_timeSinceStart = 0;
			_timeSinceLastFatigueTick = 0;
			_currentFatigueValue = startFatigueValue;
		}
		~FatigueSystem()
		{
			UnsubscribeFromEvents();
		}
		public override void Start()
		{
			_working = true;
			SubscribeToEvents();
		}
		public override void Stop()
		{
			_working = false;
			UnsubscribeFromEvents();
		}
		public override void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
		}

		public override void UnsubscribeFromEvents()
		{
			if(Ticker.I != null) Ticker.OnTick -= OnTick;
		}
		private void OnTick(Ticker.OnTickEventArgs obj)
		{
			if(!_working) return;
			_timeSinceStart += Ticker.TickInterval;
			_timeSinceLastFatigueTick += Ticker.TickInterval;
			if(!(_timeSinceStart >= _timeToStartFatigue) || !(_timeSinceLastFatigueTick >= _fatigueTickTime)) return;
			TextMessageRenderer.Instance.ShowMessage("Fatigue tick, -" + _currentFatigueValue);
			_timeSinceLastFatigueTick = 0;
			var fatigueData = new FatigueEventData(_currentFatigueValue);
			FatigueEvent.Invoke(fatigueData);
			foreach(var character in Battle.EntityRegistry.GetAllCharacters().Where(c => !c.IsDead))
			{
				character.OnFatigue(fatigueData);
			}
			_currentFatigueValue += _fatigueIncrement;
		}

	}
}