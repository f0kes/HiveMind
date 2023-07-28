using System;
using Combat;
using Combat.Spells;
using Combat.Spells.PoisonGrenade;
using DefaultNamespace.UI;
using Enums;
using GameState;
using Stats;
using UnityEngine;
using VFX;

namespace DefaultNamespace
{
	public class Entity : MonoBehaviour
	{
		private EntityData _data;

		public EntityTag Tags => _data.Tags;
		public EntityEvents Events = new EntityEvents();

		private float _currentHealthPercent = 1f;
		private ushort _team;
		private uint _level;

		private SerializableCharacterStats _stats;
		public StatDict<CS> Stats;
		[SerializeField] private ObjectGizmo _gizmoPrefab;
		public ObjectGizmo Gizmo{get; private set;}

		private bool _isDead;
		public bool IsDead => _isDead;
		public uint Level => _level;
		public ushort Team => _team;
		public float MaxHealth => Stats[CS.Health];
		public EntityData DataCopy => new(_data);
		public float CurrentHealth => _currentHealthPercent * MaxHealth;
		public float CurrentHealthPercent => _currentHealthPercent;

		private void Awake()
		{
			ChildAwake();
		}
		public void SetData(EntityData data)
		{
			_data = data;
			_level = data.Level;
			_stats = Instantiate(data.Stats);
			Stats = _stats.GetStats(Level);
		}
		private void Start()
		{
			InitGizmo();
			ChildStart();
		}

		private void InitGizmo()
		{
			Gizmo = Instantiate(_gizmoPrefab);
			var transform1 = Gizmo.transform;
			transform1.position = transform.position + Vector3.up * 4f;

			Gizmo.AttachTo(this);
		}
		protected virtual void ChildAwake()
		{
		}
		protected virtual void ChildStart()
		{
		}

		public EntityList GetTeam()
		{
			return GlobalEntities.GetTeam(_team);
		}
		public void SetTeam(ushort team)
		{
			GlobalEntities.RemoveEntityFromTeam(this);
			_team = team;
			GlobalEntities.AddToTeam(_team, this);
		}
		public void SetLevel(int level)
		{
			if(level <= 0)
			{
				Die();
			}
			level = Mathf.Clamp(level, 1, (int)GameStateController.GameData.MaxLevel);
			_level = (uint)level;
			Stats.SetLevel(_level);
		}


		protected void TakeDamage(float damage)
		{
			var newHealth = CurrentHealth - damage;
			_currentHealthPercent = newHealth / MaxHealth;
			Events.HealthChanged?.Invoke(Mathf.Max(_currentHealthPercent, 0));
			if(_currentHealthPercent <= 0)
			{
				Die();
			}
		}
		public void TakeDamage(Damage damage)
		{
			float value = damage.Value;
			float armor = Stats[CS.Armor];
			if(armor < 1f)
			{
				armor = 1f;
			}
			value /= armor;
			TakeDamage(value);
		}
		public void Die()
		{
			_isDead = true;
			Events.Death?.Invoke(this);
			gameObject.SetActive(false);
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
			Stats.GetStat(CS.Health).SetBaseValue(maxHealth);
			Events.HealthChanged?.Invoke(_currentHealthPercent);
		}

		public void ApplyNewEffect(Entity owner, BaseSpell source, BaseEffect effect, float duration)
		{
			effect.ApplyEffect(owner, this, source, duration);
		}


		public void TakeHeal(Heal heal)
		{
			var newHealth = Mathf.Clamp(CurrentHealth + heal.Value, 0, MaxHealth);
			_currentHealthPercent = Mathf.Clamp01(newHealth / MaxHealth);
			Events.HealthChanged?.Invoke(_currentHealthPercent);
		}


		public void LevelUp(int levelIncreaseAmount)
		{
			SetLevel((int)Level + levelIncreaseAmount);
			VFXSystem.I.PlayEffectFollow(VFXSystem.Data.LevelUpEffect, transform);
		}
	}
}