﻿using DefaultNamespace;
using Enums;
using Events;
using Events.Implementations;
using UnityEngine;

namespace Combat
{
	public static class BattleProcessor
	{
		//todo: redirect damage to another target

		public static void ProcessHit(Entity attacker, Entity target, Damage damage)
		{
			damage.Target = target;
			damage.Source = attacker;

			CritCheck(attacker, target, damage);

			attacker.Events.BeforeDamageDealt?.Invoke(target, damage);
			target.Events.BeforeDamageReceived?.Invoke(attacker, damage);

			target.TakeDamage(damage);
			GameEvent<Damage>.Invoke(damage);

			attacker.Events.AfterDamageDealt?.Invoke(target, damage);
			target.Events.AfterDamageReceived?.Invoke(attacker, damage);

			attacker.Events.HitLanded?.Invoke(target);
			target.Events.HitReceived?.Invoke(attacker);
		}
		private static void CritCheck(Entity attacker, Entity target, Damage damage)
		{
			var critChance = attacker.Stats[CS.CritChance];
			var critDamage = attacker.Stats[CS.CritDamage];
			if(critChance > Random.value)
			{
				damage.Value *= critDamage;
				GameEvent<CritData>.Invoke(new CritData { Damage = damage, CritChance = critChance, CritDamage = critDamage });
			}
		}

		public static void ProcessHit(Damage damage)
		{
			var attacker = damage.Source;
			var target = damage.Target;
			ProcessHit(attacker, target, damage);
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