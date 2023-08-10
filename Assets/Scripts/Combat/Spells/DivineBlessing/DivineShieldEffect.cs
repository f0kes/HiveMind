using DefaultNamespace;
using Enums;
using UnityEngine;
using VFX;

namespace Combat.Spells.DivineBlessing
{
	public class DivineShieldEffect : BaseEffect
	{
		private float _health;
		private VFXEffect _vfxEffect;
		private float _maxHealth = 100;
		protected override void PopulateParams()
		{
		}
		public void SetMaxHealth(float maxHealth)
		{
			_maxHealth = maxHealth;
		}

		public override void OnCreated()
		{
			base.OnCreated();
			_health = _maxHealth;
			_vfxEffect = VFXSystem.I.PlayEffectFollow(VFXSystem.Data.DivingShieldEffect, Target.transform);
		}
		


		public override void OnDestroyed()
		{
			base.OnDestroyed();
			_vfxEffect.Stop();
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			Target.Events.BeforeDamageReceived += TargetOnBeforeDamageReceived;
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			Target.Events.BeforeDamageReceived -= TargetOnBeforeDamageReceived;
		}
		
		private void TargetOnBeforeDamageReceived(Entity attacker, Damage damage)
		{
			base.OnBeforeDamageReceived(attacker, damage);
			if(damage.Value >= _health)
			{
				Remove();
			}
			else
			{
				_health -= damage.Value;
			}
			damage.Value = 0;
		}
	}
}