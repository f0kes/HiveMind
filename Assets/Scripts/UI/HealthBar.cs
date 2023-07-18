using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
	public class HealthBar : MonoBehaviour
	{
		[SerializeField] private  Image  _healthBarFill;
		public void SetEntity(Entity entity)
		{
			_healthBarFill.fillAmount = entity.CurrentHealthPercent;
			entity.Events.HealthChanged += OnHealthChanged;
		}

		private void OnHealthChanged(float obj)
		{
			_healthBarFill.fillAmount = obj;
		}
	}
}