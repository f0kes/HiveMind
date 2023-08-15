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
			if(_target.gameObject.activeInHierarchy)
			{
				Effect.gameObject.SetActive(true);
				Effect.transform.position = _target.position;
			}
			else
			{
				Effect.gameObject.SetActive(false);
			}
		}
	}
}