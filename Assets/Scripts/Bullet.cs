using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(Rigidbody))]
	public class Bullet : MonoBehaviour
	{
		[SerializeField] private float _speed = 40f;
		[SerializeField] private float _lifeTime = 1f;
		[SerializeField] private MeshRenderer _renderer;
		private float _currentLifeTime = 0f;
		private float _startAlpha;
		public float Damage{get; private set;} = 10;
		private bool _isInitialized;
		private ushort _team;
		private Rigidbody _rigidbody;
		private Entity _owner;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}
		private void Start()
		{
			_renderer.material = Instantiate(_renderer.material);
			_startAlpha = _renderer.material.color.a;
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
			//if(!_isInitialized)
			//	return;
			//_rigidbody.velocity = transform.forward * _speed;

			_currentLifeTime += Time.deltaTime;
			var lerpedAlpha = Mathf.Lerp(_startAlpha, 0f, _currentLifeTime / _lifeTime);
			var material = _renderer.material;
			material.color = new Color(material.color.r, material.color.g,
				material.color.b, lerpedAlpha);

			if(!(_currentLifeTime >= _lifeTime)) return;
			Destroy(gameObject);
		}

		private void OnTriggerEnter(Collider collision)
		{
			return; //TODO: remove
			if(!_isInitialized) return;
			var damaged = collision.gameObject.GetComponent<Entity>();
			if(damaged != null && damaged.Team != _team)
			{
				_owner.Events.BulletHit?.Invoke(damaged);
			}

			if(damaged != null && damaged.Team == _team)
			{
				return;
			}

			if(!collision.gameObject.CompareTag("Bullet") &&
			   !collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
			{
				Destroy(gameObject);
			}
		}
	}
}