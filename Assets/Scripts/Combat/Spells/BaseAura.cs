using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using Enums;
using GameState;
using Stats;
using UnityEngine;
using VFX;

namespace Combat.Spells
{
	public abstract class BaseAura : BaseSpell
	{
		
		private HashSet<Entity> _targets = new HashSet<Entity>();
		
		public override void OnTick(Ticker.OnTickEventArgs obj)
		{
			base.OnTick(obj);
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

		protected override void OnActivated()
		{
			base.OnActivated();
			Debug.Log("Aura activated");
			
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			RemoveFromAll();
		}
		private void RemoveFromAll()
		{
			foreach(var target in _targets)
			{
				RemoveAura(target);
			}
			_targets.Clear();
		}

		protected abstract HashSet<Entity> GetTargets();

		protected abstract void ApplyAura(Entity target);

		protected abstract void RemoveAura(Entity target);

	}
}