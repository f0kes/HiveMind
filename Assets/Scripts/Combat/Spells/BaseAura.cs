using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Enums;
using GameState;
using Stats;
using UnityEngine;

namespace Combat.Spells
{
	public abstract class BaseAura : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _duration;
		private HashSet<Entity> _targets = new HashSet<Entity>();
		private bool _active;
		protected override void PopulateParams()
		{
			AddParam(CS.AuraDuration, _duration);
		}

		public override void OnTick(Ticker.OnTickEventArgs obj)
		{
			base.OnTick(obj);
			if(!_active) return;
			var oldTargets = _targets;
			var newTargets = GetTargets();
			foreach(var oldTarget in oldTargets.Where(oldTarget => !newTargets.Contains(oldTarget))) 
			{
				RemoveAura(oldTarget);
			}
			foreach(var newTarget in newTargets.Where(newTarget => !oldTargets.Contains(newTarget)))
			{
				ApplyAura(newTarget);
			}
			_targets = newTargets;
		}
		

		public override void OnDestroyed()
		{
			base.OnDestroyed();
			foreach(var target in _targets)
			{
				RemoveAura(target);
			}
		}

		protected abstract HashSet<Entity> GetTargets();

		protected abstract void ApplyAura(Entity target);

		protected abstract void RemoveAura(Entity target);

	}
}