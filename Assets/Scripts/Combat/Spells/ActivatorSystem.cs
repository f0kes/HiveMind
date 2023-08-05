using System.Collections.Generic;
using Combat.Battle;
using GameState;

namespace Combat.Spells
{
	public class ActivatorSystem : BattleSystem, IActivator
	{
		private readonly Dictionary<IActivatable, float> _temporary = new();
		private readonly List<IActivatable> _permanent = new();
		public ActivatorSystem(IBattle battle) : base(battle)
		{
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
			foreach(var active in new List<IActivatable>(_temporary.Keys))
			{
				if(_temporary[active] <= 0)
				{
					Deactivate(active);
				}
				else
				{
					_temporary[active] -= Ticker.TickInterval;
				}
			}
		}
		public void Activate(IActivatable activatable)
		{
			if(Activated(activatable))
			{
				ResetTime(activatable);
				return;
			}
			activatable.Activate();
			if(activatable.IsPermanent())
			{
				_permanent.Add(activatable);
				activatable.Activate();
			}
			else
			{
				_temporary.Add(activatable, activatable.GetLifetime());
			}
		}
		private void ResetTime(IActivatable activatable)
		{
			if(_temporary.ContainsKey(activatable))
			{
				_temporary[activatable] = activatable.GetLifetime();
			}
		}
		private bool Activated(IActivatable activatable)
		{
			return _temporary.ContainsKey(activatable) || _permanent.Contains(activatable);
		}
		public void Deactivate(IActivatable activatable)
		{
			if(_temporary.ContainsKey(activatable))
			{
				_temporary.Remove(activatable);
			}
			else if(_permanent.Contains(activatable))
			{
				_permanent.Remove(activatable);
			}
			activatable.Deactivate();
		}
	}
}