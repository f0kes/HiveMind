using Characters;
using DefaultNamespace;

namespace Combat.Spells.GenericStun
{
	public class GenericStunEffect : BaseEffect
	{
		protected override void PopulateParams()
		{
			base.PopulateParams();
		}

		public override void ApplyEffect(Entity owner, Entity target, BaseSpell source, float duration)
		{
			base.ApplyEffect(owner, target, source, duration);
			if(target is Character character)
			{
				character.Break();
			}
		}
	}
}