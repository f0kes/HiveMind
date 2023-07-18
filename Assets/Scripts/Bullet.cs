using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(Rigidbody))]
	public class Bullet : MonoBehaviour
	{
		[SerializeField] private float _speed = 40f;
		[SerializeField] private float _lifeTime = 1f;

		private float _currentLifeTime = 0f;
		public float Damage { get; private set; } = 10;
		private bool _isInitialized;
		private ushort _team;
		private Rigidbody _rigidbody;
		private Entity _owner;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public void Init(Entity owner, float damage, ushort team)
		{
			_owner = owner;
			Damage = damage;
			_team = team;
			_isInitialized = true;
		}

		private void Update()
		{
			if (!_isInitialized)
				return;
			_rigidbody.velocity = transform.forward * _speed;

			_currentLifeTime += Time.deltaTime;
			if (_currentLifeTime >= _lifeTime)
			{
				Destroy(gameObject);
			}
		}

		private void OnTriggerEnter(Collider collision)
		{
			if (!_isInitialized) return;
			var damaged = collision.gameObject.GetComponent<Entity>();
			if (damaged != null && damaged.Team != _team)
			{
				Debug.Log("Bullet hit");
				_owner.Events.BulletHit?.Invoke(damaged);
			}

			if (damaged != null && damaged.Team == _team)
			{
				return;
			}

			if (!collision.gameObject.CompareTag("Bullet") &&
			    !collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
			{
				Destroy(gameObject);
			}
		}
	}
}