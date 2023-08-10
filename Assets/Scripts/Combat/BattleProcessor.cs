using DefaultNamespace;
using Enums;
using Events;
using Events.Implementations;
using UI;
using UnityEngine;
using VFX;

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
			var random = Random.value;
			if(critChance > random)
			{
				damage.Value *= critDamage;
				string msg = $"Crit, chance: {critChance}, roll: {random}, damage: {critDamage}!";
				Debug.Log(msg);
				GameEvent<CritData>.Invoke(new CritData { Damage = damage, CritChance = critChance, CritDamage = critDamage });
				VFXSystem.I.PlayBuffPopup(VFXSystem.Data.OnCritNotification, BuffPopup.PopupType.Neutral, target.transform.position);
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
			VFXSystem.I.PlayEffectFollow(VFXSystem.Data.HealEffect, target.transform, 1f);
		}
	}
}