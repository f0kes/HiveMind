using System;
using DefaultNamespace;

namespace Combat.Spells
{
	public interface IProjectile
	{
		public event Action<Projectile> OnProjectileHit;
		public event Action<Projectile> OnProjectileTick;

		public void AddTargetFilter(EntityFilter targetingFunction);
	}
}