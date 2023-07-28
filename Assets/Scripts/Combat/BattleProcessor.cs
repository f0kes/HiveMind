using DefaultNamespace;
using Events;

namespace Combat
{
	public static class BattleProcessor
	{
		public static void ProcessHit(Entity attacker, Entity target, Damage damage)
		{
			damage.Target = target;
			damage.Source = attacker;

			attacker.Events.BeforeDamageDealt?.Invoke(target, damage);
			target.Events.BeforeDamageReceived?.Invoke(attacker, damage);

			target.TakeDamage(damage);
			GameEvent<Damage>.Invoke(damage);

			attacker.Events.AfterDamageDealt?.Invoke(target, damage);
			target.Events.AfterDamageReceived?.Invoke(attacker, damage);

			attacker.Events.HitLanded?.Invoke(target);
			target.Events.HitReceived?.Invoke(attacker);
		}
		public static void ProcessHeal(Entity healer, Entity target, Heal heal)
		{
			heal.Target = target;
			heal.Source = healer;
			target.TakeHeal(heal);
			GameEvent<Heal>.Invoke(heal);
		}
	}
}