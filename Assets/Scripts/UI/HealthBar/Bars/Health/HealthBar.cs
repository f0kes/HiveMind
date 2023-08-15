using Characters;
using DefaultNamespace;
using Events.Implementations;
using GameState;
using Player;
using TMPro;
using UI.HealthBars.Bars.Charge;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class HealthBar : MonoBehaviour
	{
		[SerializeField] private ManaBar _manaBar;
		[SerializeField] private Image _healthBarFill;
		[SerializeField] private Image _icon;
		[SerializeField] private RectTransform _damageBarTemplate;
		[SerializeField] private RectTransform _damageBarContainer;
		[SerializeField] private TextMeshProUGUI _levelText;
		[SerializeField] private Color _playerColor = Color.green;
		[SerializeField] private Color _enemyColor = Color.red;
		[SerializeField] private Color _controlledColor = Color.yellow;
		[SerializeField] private float _offset = 1f;
		[SerializeField] private ChargeBar _chargeBar;

		private Camera _camera;
		private float _previousHealthPercent = 1f;
		private Entity _entity;
		private float _maxHealth;
		private static readonly int CurrentMaxHealth = Shader.PropertyToID("_CurrentMaxHealth");
		private RectTransform _rectTransform;

		private void Awake()
		{
			_healthBarFill.material = Instantiate(_healthBarFill.material);
			_rectTransform = GetComponent<RectTransform>();
		}
		public void SetEntity(Entity entity)
		{
			_entity = entity;
			_healthBarFill.fillAmount = entity.CurrentHealthPercent;
			_icon.sprite = entity.DataCopy.Icon;
			entity.Events.HealthChanged += OnHealthChanged;
			entity.Events.Death += OnDeath;
			CharacterSwappedEvent.Subscribe(OnCharacterSwapped);
			var character = entity as Character;
			InitManaBar(character);
			InitChargeBar(character);
			_camera = Camera.main;

			if(character != null && character.ControlsProvider == InputHandler.Instance)
				SetColor(_controlledColor);
			else if(entity.Team == 0)
				SetColor(_playerColor);
			else
				SetColor(_enemyColor);
			
		}
		private void InitManaBar(Character character)
		{
			if(character == null) return;
			_manaBar.SetCharacter(character);
		}
		private void InitChargeBar(Character character)
		{
			if(character == null) return;
			_chargeBar.SetChargable(character.ActiveSpell, GameStateController.ChargeSystem);
			_chargeBar.SetManaBar(_manaBar);
		}



		private void OnCharacterSwapped(CharacterSwappedData data)
		{
			if(data.NewCharacter == _entity && _entity.Team == 0) //todo: check if team is local player
				SetColor(_controlledColor);
			else if(data.OldCharacter == _entity)
			{
				if(_entity.Team == 0)
					SetColor(_playerColor);
				else
					SetColor(_enemyColor);
			}
		}

		private void Update()
		{
			if(_entity == null) return;
			SetMaxHealth(_entity.MaxHealth);
			SetLevel(_entity.Level);

			if(_camera == null) return;
			var pos = _entity.transform.position;
			pos.y += _offset;
			//canvas is in screen space camera
			var viewportPos = _camera.WorldToViewportPoint(pos);
			_rectTransform.anchorMin = viewportPos;
			_rectTransform.anchorMax = viewportPos;
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
			gameObject.SetActive(false);
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

		private void OnDestroy()
		{
			if(_entity != null)
			{
				_entity.Events.HealthChanged -= OnHealthChanged;
				_entity.Events.Death -= OnDeath;
			}
			CharacterSwappedEvent.Unsubscribe(OnCharacterSwapped);
		}
	}
}