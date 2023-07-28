using System;
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
		public event Action<BaseEffect> Applied;
		public event Action<BaseEffect> Destroyed;


		protected BaseSpell SourceSpell;
		public bool Stackable = true;
		
		public float TimeLeft{get; protected set;}

		public virtual void ApplyEffect(Entity owner, Entity target, BaseSpell source, float duration)
		{
			if(!Stackable)
			{
				var otherEffect = target.GetEffectOfType(this);
				if(otherEffect != null)
				{
					otherEffect.SetTimeLeft(duration);
					otherEffect.OnCreated();
					Remove();
				}
			}
			SetOwner(owner);
			Target = target;
			SourceSpell = source;
			SetTimeLeft(duration);
			OnCreated();
			Applied?.Invoke(this);
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
				Remove();
			}
		}
		public virtual void Remove()
		{
			Destroyed?.Invoke(this);
			OnDestroyed();
		}
	}
}