using System;
using DefaultNamespace;
using GameState;
using UnityEngine;

namespace Characters
{
	[RequireComponent(typeof(Rigidbody))]
	public class CharacterMover : MonoBehaviour
	{
		[SerializeField] private float _moveSpeed = 5f;

		[SerializeField] private float _acceleration = 5f;
		[SerializeField] private float _deceleration = 5f;
		//[SerializeField] private float _rotationOffset = 35f;

		private Vector3 _lookAt = new Vector2(0, 0);
		private Vector2 _moveDirection = new Vector2(0f, 0f);

		private Rigidbody _rigidbody;

		public Vector2 Movement => _moveDirection;
		public Entity CursorTarget{get; private set;}

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}
		private void OnEnable()
		{
			Ticker.OnTick += Tick;
		}
		private void OnDisable()
		{
			Ticker.OnTick -= Tick;
		}

		private void Tick(Ticker.OnTickEventArgs obj)
		{
			Move();
		}

		private void Update()
		{
		}

		public void SetInput(Vector2 moveDirection, Vector3 lookAt)
		{
			_moveDirection = moveDirection;
			_lookAt = lookAt;
		}
		public Entity GetCursorTarget()
		{
			return CursorTarget;
		}
		public void SetCursorTarget(Entity target)
		{
			CursorTarget = target;
		}
		public Vector3 GetCursor()
		{
			return _lookAt;
		}


		private void Move()
		{
			_moveDirection.Normalize();
			Vector3 dir = new Vector3(_moveDirection.x, 0, _moveDirection.y) * _acceleration;
			if(dir == Vector3.zero)
			{
				AccelerateTowards(-_rigidbody.velocity.normalized, _deceleration);
			}
			else
			{
				AccelerateTowards(dir, _acceleration);
			}


			var transform1 = transform;
			var direction = _lookAt - transform1.position;
			direction.y = 0;
			transform1.forward = direction;
		}

		private void AccelerateTowards(Vector3 dir, float acceleration)
		{
			var rbVelocity = _rigidbody.velocity;
			var accel = dir * acceleration;
			var newVel = rbVelocity + accel * Time.deltaTime;

			rbVelocity = rbVelocity.magnitude * newVel.normalized;

			_rigidbody.velocity = newVel.magnitude <= _moveSpeed ? newVel : rbVelocity;
		}


	}
}