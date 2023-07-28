using UnityEngine;

namespace VFX
{
	public class VFXEffect : MonoBehaviour
	{

		[SerializeField] private float _duration = 1f;
		private VFXBehaviour _behaviour;

		public float Duration => _duration;

		public void Follow(Transform target)
		{
			_behaviour = new VFXBehaviours.VFXFollow(this, target);
		}
		public void RepeatMovement(Transform target)
		{
			_behaviour = new VFXBehaviours.VFXRepeatMovement(this, target);
		}
		private void Update()
		{
			_behaviour.Execute();
		}

		public void Stop()
		{
			Destroy(gameObject);
		}
	}
}