using System.Linq;
using DefaultNamespace;
using Enums;
using GameState;
using Misc;
using Stats;
using UnityEngine;
using VFX;
using VFX.VFXBehaviours;

namespace Combat.Spells.Redirection
{
	public class RedirectionSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _damageMultiplier;
		[SerializeField] private MinMaxStatRange _duration;
		protected override void PopulateParams()
		{
			AddParam(CS.RedirectionDamageMultiplier, _damageMultiplier);
			AddParam(CS.RedirectionDuration, _duration);
		}

		public override void OnBeforeDamageReceived(Entity attacker, Damage damage)
		{
			base.OnBeforeDamageReceived(attacker, damage);
			if(!damage.Redirecrable) return;
			var owner = GetOwnerCharacter();
			if(owner == null) return;
			if(owner.IsDead) return;
			if(attacker.GetTeam() != owner.GetTeam()) return;
			var damageMultiplier = GetParam(CS.RedirectionDamageMultiplier);
			var damageVal = damage.Value * damageMultiplier;
			var randomEnemy = GameStateController.Battle.EntityRegistry.GetAllCharacters().Where(c => c.GetTeam() != owner.GetTeam()).Random();
			if(randomEnemy == null)
			{
				Debug.LogError("No enemy found");
				return;
			}
			damage.Value = 0;
			var newDamage = new Damage
			{
				Source = owner,
				Target = randomEnemy,
				Spell = this,
				Value = damageVal,
				Redirecrable = false
			};
			BattleProcessor.ProcessHit(newDamage);

			//var effect = VFXSystem.I.PlayMultiplePointEffect(VFXSystem.Data.RedirectionEffect);
			//effect.SetEffectBehaviour(new VFXFollow(effect, owner.transform), 0);
			//effect.SetEffectBehaviour(new VFXFollow(effect, randomEnemy.transform), 1);
		}

		public override bool IsPermanent()
		{
			return false;
		}

		public override float GetLifetime()
		{
			return GetParam(CS.RedirectionDuration);
		}
	}
}