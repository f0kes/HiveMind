using UnityEngine;

namespace VFX.VFXBehaviours
{
	public class VFXFollow : VFXBehaviour
	{
		private Transform _target;
		public VFXFollow(VFXEffect effect, Transform target) : base(effect)
		{
			_target = target;
		}
		public override void Execute()
		{
			Effect.transform.position = _target.position;
		}
	}
}