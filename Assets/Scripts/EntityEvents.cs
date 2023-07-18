using System;
using Combat;

namespace DefaultNamespace
{
	public class EntityEvents
	{
		public Action<Entity, Damage> BeforeDamageReceived;
		public Action<float> HealthChanged;
		public Action<Entity> Death;
		public Action<Entity> Ressurect;

		public Action<Entity, Damage> AfterDamageReceived;
		public Action<Entity> HitReceived;
		public Action<Entity, Damage> BeforeDamageDealt;
		public Action<Entity, Damage> AfterDamageDealt;

		public Action<Entity> BulletHit;
		public Action<Entity> HitLanded;
	}
}