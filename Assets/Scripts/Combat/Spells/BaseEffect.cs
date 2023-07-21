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
		protected BaseSpell SourceSpell;
		public float TimeLeft{get; protected set;}

		public virtual void ApplyEffect(Entity owner, Entity target, BaseSpell source, float duration)
		{
			SetOwner(owner);
			Target = target;
			SourceSpell = source;
			SetTimeLeft(duration);
			OnCreated();
		}
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