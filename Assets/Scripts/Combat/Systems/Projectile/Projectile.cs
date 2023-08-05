using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using DefaultNamespace;
using GameState;
using UnityEngine;

namespace Combat.Spells
{
	[RequireComponent(typeof(Rigidbody), typeof(Collider))]
	public class Projectile : MonoBehaviour, IProjectile
	{
		public event Action<Projectile> OnProjectileHit;
		public event Action<Projectile> OnProjectileTick;

		private List<EntityFilter> _targetFilters = new List<EntityFilter>();



		public ProjectileData Data;

		private float _distanceTraveled;
		private Rigidbody _rigidbody;
		private void Awake()
		{
			Ticker.OnTick += OnTick;
			_rigidbody = GetComponent<Rigidbody>();
		}
		private void OnDestroy()
		{
			Ticker.OnTick -= OnTick;
		}

		private void OnTick(Ticker.OnTickEventArgs obj)
		{
			OnProjectileTick?.Invoke(this);
			Move();
		}
		public void AddTargetFilter(EntityFilter targetingFunction)
		{
			_targetFilters.Add(targetingFunction);
		}
		protected virtual void Move()
		{
			_rigidbody.velocity = Data.Velocity;
			_distanceTraveled += Data.Velocity.magnitude * Ticker.TickInterval;
			if(_distanceTraveled >= Data.Range)
			{
				Destroy(gameObject);
			}
		}
		private void OnTriggerEnter(Collider other)
		{
			var target = other.gameObject.GetComponent<Entity>();
			if(target == null || !CheckTargetFilters(Data.Owner, target)) return;
			Data.Target = target;
			OnProjectileHit?.Invoke(this);
		}
		private bool CheckTargetFilters(Entity owner, Entity target)
		{
			return _targetFilters.All(filter => filter(owner, target));
		}
	}
}