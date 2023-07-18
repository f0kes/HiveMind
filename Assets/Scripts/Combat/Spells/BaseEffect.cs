using System.Collections.Generic;
using DefaultNamespace;
using Enums;
using GameState;
using Stats.Structures;
using UnityEngine;

namespace Combat.Spells
{
	public abstract class BaseEffect : BaseSpell
	{
		public BaseSpell SourceSpell;
		public float TimeLeft{get; protected set;}
		

		public override float GetParam(CS statName)
		{
			return base.ContainsParam(statName) ? base.GetParam(statName) : SourceSpell.GetParam(statName);
		}
		
		public void SetTimeLeft(float timeLeft)
		{
			TimeLeft = timeLeft;
		}

		public override void OnTick(Ticker.OnTickEventArgs obj)
		{
			base.OnTick(obj);
			TimeLeft -= Ticker.TickInterval;
			if(TimeLeft <= 0)
			{
				OnDestroyed();
			}
		}
	}
}