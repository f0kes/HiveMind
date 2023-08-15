using System;
using Combat.ChargeSystem;
using Combat.Systems.ChargeSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HealthBars.Bars.Charge
{
	public class ChargeBar : MonoBehaviour
	{
		[SerializeField] private Image _chargeBarFill;
		[SerializeField] private Image _outline;
		private IChargable _chargable;
		private ICharger _charger;
		private ManaBar _manaBar;

		private Color _mainColor;
		private Color _outlineColor;
		private float _currentGlow = 1f;

		private static readonly int CurrentMaxHealth = Shader.PropertyToID("_CurrentMaxHealth");

		public void SetChargable(IChargable chargable, ICharger charger)
		{
			_chargable = chargable;
			_charger = charger;
			_charger.OnChargeStarted += OnChargeStarted;
			_charger.OnCharged += OnCharged;
			_charger.OnChargeLost += OnChargeLost;
		}
		public void SetManaBar(ManaBar manaBar)
		{
			_manaBar = manaBar;
		}
		private void Start()
		{
			_mainColor = _chargeBarFill.color;
			_outlineColor = _outline.color;

			_chargeBarFill.material = Instantiate(_chargeBarFill.material);
			_outline.material = Instantiate(_outline.material);

			_chargeBarFill.material.color = _mainColor;
			_outline.material.color = _outlineColor;
			Disable();
		}
		private void Update()
		{
			if(_chargable == null) return;
			SetMaxCharge(_chargable.GetMaxCharge());
			SetCharge(_charger.GetCharge(_chargable));
		}
		public void Enable()
		{
			_chargeBarFill.enabled = true;
			_outline.enabled = true;
			_manaBar.Disable();
			Debug.Log("Enable");
		}
		public void Disable()
		{
			_outline.enabled = false;
			_chargeBarFill.enabled = false;
			_manaBar.Enable();
		}
		public void Glow(float val)
		{
			_currentGlow = val;
			UpdateColor();
		}
		public void DisableGlow()
		{
			_currentGlow = 1f;
			UpdateColor();
		}
		private void UpdateColor()
		{
			_chargeBarFill.material.color = _mainColor * _currentGlow;
			_outline.material.color = _outlineColor * _currentGlow;
		}

		private void OnChargeLost(IChargable obj)
		{
			if(obj != _chargable) return;
			Disable();
		}

		private void OnCharged(IChargable obj)
		{
			if(obj != _chargable) return;
			Glow(10f);
		}

		private void OnChargeStarted(IChargable obj)
		{
			if(obj != _chargable) return;
			Enable();
			DisableGlow();
		}
		private void SetMaxCharge(float maxCharge)
		{
			_chargeBarFill.material.SetFloat(CurrentMaxHealth, maxCharge);
		}
		private void SetCharge(float charge)
		{
			_chargeBarFill.fillAmount = charge / _chargable.GetMaxCharge();
		}
	}
}