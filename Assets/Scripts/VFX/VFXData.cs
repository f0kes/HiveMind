using UnityEngine;

namespace VFX
{
	[CreateAssetMenu(fileName = "VFXData", menuName = "VFX/VFXData", order = 0)]
	public class VFXData : ScriptableObject
	{
		public VFXEffect LevelUpEffect;
		public VFXEffect ExecuteEffect;
		public VFXEffect DivingShieldEffect;
		public VFXEffect HellfireEffect;
		public VFXMultiplePointEffect RedirectionEffect;
		public VFXEffect Aura;
		public VFXEffect HealEffect;
		public VFXEffect DeathEffect;

		public BuffPopup DamageBuff;
		public BuffPopup CritBuff;
		public BuffPopup TriggerBuff;
		public BuffPopup OnCritNotification;

	}
}