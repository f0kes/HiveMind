using System;

namespace Events
{
	public abstract class GameEvent<T> : IResettable where T : struct
	{
		private static event Action<T> OnEvent;
		//todo: subscribe to events, make virtual methods for each event
		public GameEvent()
		{
			OnEvent = null;
		}
		public GameEvent(EventResetter resetter)
		{
			OnEvent = null;
			resetter.Add(this);
		}
		public static void Subscribe(Action<T> action)
		{
			OnEvent += action;
		}
		public static void Unsubscribe(Action<T> action)
		{
			OnEvent -= action;
		}
		public static void Invoke(T arg)
		{
			OnEvent?.Invoke(arg);
		}
		public void Reset()
		{
			OnEvent = null;
		}
	}
}