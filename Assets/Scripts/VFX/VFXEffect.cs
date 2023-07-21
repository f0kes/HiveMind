﻿using UnityEngine;

namespace VFX
{
	public class VFXEffect : MonoBehaviour
	{

		private VFXBehaviour _behaviour;
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
	}
}