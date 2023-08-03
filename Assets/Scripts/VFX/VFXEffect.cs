using System;
using UnityEngine;

namespace VFX
{
	public class VFXEffect : MonoBehaviour
	{
		public event Action<VFXEffect> OnDestroyEvent;

		[SerializeField] private float _duration = 1f;
		protected VFXBehaviour Behaviour;

		public float Duration => _duration;

		public void Follow(Transform target)
		{
			Behaviour = new VFXBehaviours.VFXFollow(this, target);
		}
		public void RepeatMovement(Transform target)
		{
			Behaviour = new VFXBehaviours.VFXRepeatMovement(this, target);
		}
		public void SetBehaviour(VFXBehaviour behaviour)
		{
			Behaviour = behaviour;
		}
		private void Update()
		{
			Behaviour?.Execute();
		}

		public void Stop()
		{
			Destroy(gameObject);
		}
		private void OnDestroy()
		{
			OnDestroyEvent?.Invoke(this);
		}
	}
}