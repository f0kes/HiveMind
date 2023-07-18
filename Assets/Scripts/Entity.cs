using System;
using Combat;
using Combat.Spells;
using Combat.Spells.PoisonGrenade;
using DefaultNamespace.UI;
using Enums;
using UnityEngine;

namespace DefaultNamespace
{
	public class Entity : MonoBehaviour
	{
		public EntityTag Tags;
		public EntityEvents Events = new EntityEvents();
		[SerializeField] private float _maxHealth;
		private float _currentHealthPercent = 1f;
		[SerializeField] private ushort _team;
		public ObjectGizmo Gizmo{get; private set;}

		private bool _isDead;
		public bool IsDead => _isDead;

		public ushort Team => _team;
		public float MaxHealth => _maxHealth;

		public float CurrentHealth => _currentHealthPercent * _maxHealth;
		public float CurrentHealthPercent => _currentHealthPercent;

		private void Awake()
		{
			EntityList.AddToTeam(_team, this);
			ChildAwake();
		}
		private void Start()
		{
			InitGizmo();
			ChildStart();
		}
		private void InitGizmo()
		{
			Gizmo = Instantiate(ObjectGizmo.Default, transform, true);
			Gizmo.transform.position = transform.position + Vector3.up * 8f;
			Gizmo.healthBar.SetEntity(this);
		}
		protected virtual void ChildAwake()
		{
		}
		protected virtual void ChildStart()
		{
		}

		public void SetTeam(ushort team)
		{
			EntityList.RemoveEntityFromTeam(this);
			_team = team;
			EntityList.AddToTeam(_team, this);
			Debug.Log(MaxHealth + " " + gameObject.name);
		}


		public void TakeDamage(float damage)
		{
			float newHealth = CurrentHealth - damage;
			_currentHealthPercent = Mathf.Clamp01(newHealth / _maxHealth);
			Events.HealthChanged?.Invoke(_currentHealthPercent);
			if(_currentHealthPercent <= 0)
			{
				_isDead = true;
				Events.Death?.Invoke(this);
				gameObject.SetActive(false);
			}
		}
		public void TakeDamage(Damage damage)
		{
			float newHealth = CurrentHealth - damage.Value;
			_currentHealthPercent = Mathf.Clamp01(newHealth / _maxHealth);
			Events.HealthChanged?.Invoke(_currentHealthPercent);
			if(_currentHealthPercent <= 0)
			{
				_isDead = true;
				Events.Death?.Invoke(this);
				gameObject.SetActive(false);
			}
		}

		public void TakeFullRestore()
		{
			_currentHealthPercent = 1f;
			Events.HealthChanged?.Invoke(_currentHealthPercent);
		}

		public void Ressurect()
		{
			gameObject.SetActive(true);
			_isDead = false;
			_currentHealthPercent = 1f;
			Events.HealthChanged?.Invoke(_currentHealthPercent);
			Events.Ressurect?.Invoke(this);
		}

		public void SetMaxHealth(float maxHealth)
		{
			_maxHealth = maxHealth;
			Events.HealthChanged?.Invoke(_currentHealthPercent);
		}

		public void ApplyNewEffect(Entity owner, BaseSpell source, BaseEffect effect, float duration)
		{
			effect.SetOwner(owner);
			effect.Target = this;
			effect.SourceSpell = source;
			effect.SetTimeLeft(duration);
			effect.OnCreated();
		}


		public void TakeHeal(Heal heal)
		{
			var newHealth = Mathf.Clamp(CurrentHealth + heal.Value, 0, _maxHealth);
			_currentHealthPercent = Mathf.Clamp01(newHealth / _maxHealth);
			Events.HealthChanged?.Invoke(_currentHealthPercent);
		}
	}
}