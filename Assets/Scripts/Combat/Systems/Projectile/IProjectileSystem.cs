using DefaultNamespace;
using UnityEngine;

namespace Combat.Spells
{
	public interface IProjectileSystem
	{
		Projectile LaunchProjectile(Projectile projectile, ProjectileData data, IProjectileHandler handler);

		void DestroyProjectile(Projectile projectile);
	}
}