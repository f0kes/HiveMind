using DefaultNamespace;
using Enums;

namespace Combat.Spells.AutoAttack
{
	public class AutoAttackSpell : BaseSpell
	{

		protected override void PopulateParams()
		{
			
		}

		public override void OnBulletHit(Entity target)
		{
			base.OnBulletHit(target);
			var damageValue = GetOwnerCharacter().Stats[CS.Damage];
			var damage = new Damage(Owner, target, this, damageValue);
			BattleProcessor.ProcessHit(Owner, target, damage);
		}
		
	}
}