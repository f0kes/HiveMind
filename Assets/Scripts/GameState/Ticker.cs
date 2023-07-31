using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameState
{
	public class Ticker : MonoBehaviour
	{
		public static Ticker I;

		public delegate void SimpleDelegate();

		public class OnTickEventArgs : EventArgs
		{
			public int Tick;
			public bool Simulating;
		}

		public class OnUpdateEventArgs : EventArgs
		{
			public float DeltaTime;
		}

		[SerializeField] private static float _tickRate = 60f;
		public static float TickInterval => 1f / _tickRate;
		private float _currentTickTime = 0f;

		private int _currentTick;

		[SerializeField] private bool _isPaused = false;

		public int CurrentTick
		{
			get => _currentTick;
			set => _currentTick = value;
		}

		public static event Action<OnTickEventArgs> OnTickStart;
		public static event Action<OnTickEventArgs> OnTick;
		public static event Action<OnTickEventArgs> OnTickEnd;
		public static event EventHandler<OnUpdateEventArgs> OnUpdate;

		public static void ResetEvents()
		{
			OnTickStart = null;
			OnTick = null;
			OnTickEnd = null;
			OnUpdate = null;
		}
		public async void InvokeInTime(Action toInvoke, float time)
		{
			float timePassed = 0;

			void UpdateDelegate(object sender, OnUpdateEventArgs obj)
			{
				timePassed += obj.DeltaTime;
			}

			OnUpdate += UpdateDelegate;
			while (timePassed < time)
			{
				await Task.Yield();
			}
			OnUpdate -= UpdateDelegate;

			toInvoke.Invoke();
		}
		public async void Schedule(SimpleDelegate method, int ticks)
		{
			var tickToWait = CurrentTick + ticks;
			while (CurrentTick < tickToWait)
			{
				await Task.Yield();
			}
			Debug.Log("Scheduled method called"); //TODO remove
			method();
		}
		public void StartGame()
		{
			// foreach(var behaviour in IdTable<NetworkBehaviour>.GetAll())
			// {
			// 	behaviour.Init();
			// }
		}

		private void Awake()
		{
			_currentTick = 0;
			if(I == null)
			{
				I = this;
				transform.SetParent(null);
				DontDestroyOnLoad(this);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Update()
		{
			_currentTickTime += Time.deltaTime;
			while (_currentTickTime >= TickInterval)
			{
				_currentTickTime -= TickInterval;
				Tick();
			}

			OnUpdate?.Invoke(this, new OnUpdateEventArgs { DeltaTime = Time.deltaTime });
		}

		public void Pause()
		{
			_isPaused = true;
		}

		public void Unpause()
		{
			_isPaused = false;
		}

		public void Tick(bool simulating = false)
		{
			if(_isPaused) return;
			_currentTick++;

			OnTickStart?.Invoke(new OnTickEventArgs { Tick = _currentTick, Simulating = simulating });
			OnTick?.Invoke(new OnTickEventArgs { Tick = _currentTick, Simulating = simulating });

			Physics.Simulate(TickInterval);
			if(!simulating)
			{
				//update input on clients
			}

			OnTickEnd?.Invoke(new OnTickEventArgs { Tick = _currentTick, Simulating = simulating });
		}


		public static float TicksToSeconds(int ticks)
		{
			return ticks * TickInterval;
		}

		public static int SecondsToTicks(float seconds)
		{
			return (int)(seconds / TickInterval);
		}
	}
}