using UnityEngine;

namespace VFX.VFXBehaviours
{
	public class VFXRepeatMovement : VFXBehaviour
	{
		private Transform _target;
		private Vector3 _offset;
		public VFXRepeatMovement(VFXEffect effect, Transform target) : base(effect)
		{
			_target = target;
			_offset = Effect.transform.position - _target.position;
		}
		public override void Execute()
		{
			Effect.transform.position = _target.position + _offset;
		}
	}
}