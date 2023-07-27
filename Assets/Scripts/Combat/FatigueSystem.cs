using System.Collections.Generic;
using Characters;
using Events.Implementations;
using GameState;
using UI;

namespace Combat
{
	public class FatigueSystem : ICombatSystem
	{
		private List<Character> _characters;

		private float _timeToStartFatigue;
		private float _fatigueTickTime;
		private int _startFatigueValue;
		private int _fatigueIncrement;

		private float _timeSinceStart;
		private float _timeSinceLastFatigueTick;
		private int _currentFatigueValue;

		private bool _working;
		public FatigueSystem(List<Character> characters, float timeToStartFatigue, float fatigueTickTime, int startFatigueValue, int fatigueIncrement)
		{
			_characters = characters;
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
		public void Start()
		{
			_working = true;
			SubscribeToEvents();
		}
		public void Stop()
		{
			_working = false;
			UnsubscribeFromEvents();
		}
		public void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
		}

		public void UnsubscribeFromEvents()
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
			foreach(var character in _characters)
			{
				character.OnFatigue(fatigueData);
			}
			_currentFatigueValue += _fatigueIncrement;
		}

	}
}