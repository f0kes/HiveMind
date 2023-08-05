using Characters;

namespace Combat.Spells
{
	public interface IProjectileHandler
	{
		void OnProjectileHit(Projectile projectile);

		void OnProjectileTick(Projectile projectile);
	}
}