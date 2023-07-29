using System.Linq;
using DefaultNamespace;
using Enums;
using Misc;
using Stats;
using UnityEngine;

namespace Combat.Spells.Redirection
{
	public class RedirectionSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _damageMultiplier;
		protected override void PopulateParams()
		{
			AddParam(CS.RedirectionDamageMultiplier, _damageMultiplier);
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
			var randomEnemy = GlobalEntities.GetAllCharacters().Where(c => c.GetTeam() != owner.GetTeam()).Random();
			if(randomEnemy == null) return;
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
		}
	}
}