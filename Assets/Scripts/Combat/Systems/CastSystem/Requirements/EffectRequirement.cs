using Combat.Spells;
using Misc;

namespace Combat.CastSystem.Requirements
{
	public class EffectRequirement<TEffect> : IRequirement<BaseSpell> where TEffect : BaseEffect
	{
		private bool _mustContain;
		public EffectRequirement(bool mustContain) : base()
		{
			_mustContain = mustContain;
		}
		public TaskResult DoesSatisfy(BaseSpell spell)
		{
			var effect = spell.GetOwnerCharacter().GetEffectOfType<TEffect>();
			if(_mustContain)
			{
				return effect != null ? TaskResult.Success : TaskResult.Failure($"Character does not have {typeof(TEffect)}");
			}
			else
			{
				return effect == null ? TaskResult.Success : TaskResult.Failure($"Character has {typeof(TEffect)} effect");
			}
		}
	}
}