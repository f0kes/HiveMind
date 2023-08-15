using System;
using UnityEngine;

namespace VFX
{
	public class VFXEffect : MonoBehaviour
	{
		private Transform _target;
		public event Action<VFXEffect> OnDestroyEvent;

		[SerializeField] private float _duration = 1f;
		protected VFXBehaviour Behaviour;
		private bool _destroyed;

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
		public void SetDuration(float duration)
		{
			_duration = duration;
		}
		protected virtual void Update()
		{
			Behaviour?.Execute();
		}

		public void Stop()
		{
			if(_destroyed) return;
			Destroy(gameObject);
		}
		private void OnDestroy()
		{
			OnDestroyEvent?.Invoke(this);
			_destroyed = true;
		}
	}
}