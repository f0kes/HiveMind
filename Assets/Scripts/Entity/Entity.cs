using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Combat.Spells;
using Combat.Spells.GenericStun;
using DefaultNamespace.UI;
using Enums;
using Events.Implementations;
using GameState;
using Stats;
using UnityEngine;
using VFX;

namespace DefaultNamespace
{
	public class Entity : MonoBehaviour
	{
		private EntityData _data;

		public HashSet<EntityTag> Tags => _data.Tags.ToHashSet();
		public EntityEvents Events = new EntityEvents();

		private float _currentHealthPercent = 1f;
		private ushort _team;
		private uint _maxLevel;
		private uint _level;
		private List<BaseEffect> _effects = new List<BaseEffect>();

		private SerializableCharacterStats _stats;
		public StatDict<CS> Stats;
		[SerializeField] private ObjectGizmo _gizmoPrefab;
		public ObjectGizmo Gizmo{get; private set;}

		private bool _isDead;
		public bool IsDead => _isDead;
		public uint MaxLevel => _maxLevel;
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
			_maxLevel = GameStateController.GameData.MaxLevel;
			_stats = Instantiate(data.Stats);
			Stats = _stats.GetStats(Level);
		}
		private void Start()
		{
			InitGizmo();
			SubscribeToEvents();
			ChildStart();
		}
		public virtual void OnDestroy()
		{
			UnSubscribeFromEvents();
		}
		private void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
		}

		private void UnSubscribeFromEvents()
		{
			Ticker.OnTick -= OnTick;
		}
		private void OnTick(Ticker.OnTickEventArgs obj)
		{
			ProcessRegen();
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

		public EntityTeam GetTeam()
		{
			return GameStateController.Battle.EntityRegistry.GetTeam(_team);
		}
		public void SetTeam(ushort team)
		{
			GameStateController.Battle.EntityRegistry.RemoveEntityFromTeam(this);
			_team = team;
			GameStateController.Battle.EntityRegistry.AddToTeam(_team, this);
		}
		public void SetMaxLevel(int maxLevel)
		{
			if(maxLevel <= 0)
			{
				maxLevel = 0;
			}
			_maxLevel = (uint)maxLevel;
		}
		public void SetLevel(int level)
		{
			if(level <= 0)
			{
				Die();
			}
			level = Mathf.Clamp(level, 1, (int)_maxLevel);
			_level = (uint)level;
			Stats.SetLevel(_level);
		}

		protected void ProcessRegen()
		{
			float regen = Stats[CS.Regen];
			regen = Mathf.Clamp01(regen);
			AddHealthPercent(regen * Ticker.TickInterval);
		}
		protected void AddHealthPercent(float percent)
		{
			_currentHealthPercent += percent;
			_currentHealthPercent = Mathf.Clamp01(_currentHealthPercent);
			Events.HealthChanged?.Invoke(_currentHealthPercent);
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
			DeathEvent.Invoke(new DeathData { Target = this });
			VFXSystem.I.PlayEffectPoint(VFXSystem.Data.DeathEffect, transform.position);
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
			_effects.Add(effect);
			effect.Destroyed += OnEffectDestroyed;
		}

		private void OnEffectDestroyed(BaseEffect obj)
		{
			_effects.Remove(obj);
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
			if(levelIncreaseAmount > 0)
			{
				VFXSystem.I.PlayEffectFollow(VFXSystem.Data.LevelUpEffect, transform);
			}
			else
			{
				//VFXSystem.I.PlayEffectFollow(VFXSystem.Data.LevelDownEffect, transform);
			}
		}

		public BaseEffect GetEffectOfType(BaseEffect effect)
		{
			return _effects.FirstOrDefault(baseEffect => baseEffect.GetType() == effect.GetType());
		}
		public BaseEffect GetEffectOfType<T>() where T : BaseEffect
		{
			return _effects.OfType<T>().FirstOrDefault();
		}
		public bool IsStunned()
		{
			return GetEffectOfType<GenericStunEffect>() != null;
		}
	}
}