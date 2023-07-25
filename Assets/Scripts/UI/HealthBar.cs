using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace.UI
{
	public class HealthBar : MonoBehaviour
	{
		[SerializeField] private Image _healthBarFill;
		[SerializeField] private Image _icon;
		[SerializeField] private RectTransform _damageBarTemplate;
		[SerializeField] private RectTransform _damageBarContainer;
		[SerializeField] private TextMeshProUGUI _levelText;
		[SerializeField] private Color _playerColor = Color.green;
		[SerializeField] private Color _enemyColor = Color.red;


		private float _previousHealthPercent = 1f;
		private Entity _entity;
		private float _maxHealth;
		private static readonly int CurrentMaxHealth = Shader.PropertyToID("_CurrentMaxHealth");

		private void Awake()
		{
			_healthBarFill.material = Instantiate(_healthBarFill.material);
		}
		public void SetEntity(Entity entity)
		{
			_entity = entity;
			_healthBarFill.fillAmount = entity.CurrentHealthPercent;
			_icon.sprite = entity.DataCopy.Icon;
			entity.Events.HealthChanged += OnHealthChanged;
			entity.Events.Death += OnDeath;
			if(entity.Team == 0)
				SetColor(_playerColor);
			else
				SetColor(_enemyColor);
		}
		private void Update()
		{
			if(_entity != null)
			{
				SetMaxHealth(_entity.MaxHealth);
				SetLevel(_entity.Level);
			}
		}
		public void SetLevel(uint level)
		{
			_levelText.text = level.ToString();
		}
		public void SetMaxHealth(float maxHealth)
		{
			_maxHealth = maxHealth;
			_healthBarFill.material.SetFloat(CurrentMaxHealth, maxHealth);
		}
		public void SetColor(Color color)
		{
			_healthBarFill.color = color;
		}
		private void OnDeath(Entity obj)
		{
			_entity.Events.HealthChanged += OnHealthChanged;
			_entity.Events.Death += OnDeath;
			Destroy(gameObject);
		}

		private void OnHealthChanged(float healthPercent)
		{
			if(_healthBarFill == null)
				return;
			bool isDamage = healthPercent < _previousHealthPercent;
			if(isDamage)
			{
				var damageBar = Instantiate(_damageBarTemplate, _damageBarContainer);
				damageBar.gameObject.SetActive(true);
				var barRect = _healthBarFill.rectTransform.rect;
				damageBar.anchoredPosition = new Vector2(barRect.width * healthPercent, 0f);
				damageBar.sizeDelta = new Vector2(barRect.width * (_previousHealthPercent - healthPercent), damageBar.sizeDelta.y);
			}
			_healthBarFill.fillAmount = healthPercent;
			_previousHealthPercent = healthPercent;
		}
	}
}