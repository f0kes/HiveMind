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
		protected override void PopulateParams()
		{
		}

		public override void OnCreated()
		{
			base.OnCreated();
			_health = GetParam(CS.DivineShieldHP);
			_vfxEffect = VFXSystem.I.PlayEffectFollow(VFXSystem.Data.DivingShieldEffect, Target.transform);
		}

		public override void OnDestroyed()
		{
			base.OnDestroyed();
			_vfxEffect.Stop();
		}

		public override void OnBeforeDamageReceived(Entity attacker, Damage damage)
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