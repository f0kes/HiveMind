using System;
using UnityEngine;

namespace DefaultNamespace
{
	public class Entity : MonoBehaviour
	{
		public event Action<float> OnHealthChanged;
		public event Action<Entity> OnDeath;
		public event Action<Entity> OnRessurect;
		[SerializeField] private float _maxHealth;
		private float _currentHealthPercent = 1f;
		[SerializeField] private ushort _team;
		public ushort Team => _team;
		public float MaxHealth => _maxHealth;

		public float CurrentHealth => _currentHealthPercent * _maxHealth;
		public float CurrentHealthPercent => _currentHealthPercent;


		protected void Awake()
		{
			EntityList.AddToTeam(_team, this);
			ChildAwake();
		}

		protected virtual void ChildAwake()
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
			OnHealthChanged?.Invoke(_currentHealthPercent);
			if (_currentHealthPercent <= 0)
			{
				OnDeath?.Invoke(this);
				gameObject.SetActive(false);
			}
		}

		public void TakeFullRestore()
		{
			_currentHealthPercent = 1f;
			OnHealthChanged?.Invoke(_currentHealthPercent);
		}

		public void Ressurect()
		{
			gameObject.SetActive(true);
			_currentHealthPercent = 1f;
			OnHealthChanged?.Invoke(_currentHealthPercent);
			OnRessurect?.Invoke(this);
		}

		public void SetMaxHealth(float maxHealth)
		{
			Debug.Log(name + " " + maxHealth);
			_maxHealth = maxHealth;
			OnHealthChanged?.Invoke(_currentHealthPercent);
		}
	}
}