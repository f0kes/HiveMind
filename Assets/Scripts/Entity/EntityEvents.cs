using System;
using Characters;
using Combat;
using Combat.Spells;


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

		public Action<BaseSpell> SpellCasted;
		public Action<Character> SwappedWithCharacter;
	}
}