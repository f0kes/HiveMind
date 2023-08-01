using System;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
	public abstract class GameEvent<T> : IResettable
	{
		private static event Action<T> OnEvent;
		//todo: subscribe to events, make virtual methods for each event

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
			Debug.Log("Reset");
			OnEvent = null;
		}
	}
}