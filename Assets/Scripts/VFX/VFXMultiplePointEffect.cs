using System.Collections.Generic;
using UnityEngine;

namespace VFX
{
	public class VFXMultiplePointEffect : VFXEffect
	{
		[SerializeField] private List<VFXEffect> _subEffects;
		public void SetEffectBehaviour(VFXBehaviour behaviour, int index)
		{
			if(index < _subEffects.Count)
				_subEffects[index].SetBehaviour(behaviour);
		}

	}
}