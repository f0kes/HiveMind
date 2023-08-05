using Combat.Battle;
using GameState;
using UnityEngine;

namespace Combat.Spells
{
	public class ProjectileSystem : BattleSystem, IProjectileSystem
	{
		public ProjectileSystem(IBattle battle) : base(battle)
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
		}

		public Projectile LaunchProjectile(Projectile projectile, ProjectileData data, IProjectileHandler handler)
		{
			var instance = Object.Instantiate(projectile);
			instance.Data = data;
			instance.OnProjectileHit += handler.OnProjectileHit;
			instance.OnProjectileTick += handler.OnProjectileTick;
			instance.transform.position = data.StartPosition;
			instance.transform.rotation = Quaternion.LookRotation(data.Velocity);
			instance.gameObject.SetActive(true);
			return instance;
		}

		public void DestroyProjectile(Projectile projectile)
		{
			Object.Destroy(projectile.gameObject);
		}
	}
}