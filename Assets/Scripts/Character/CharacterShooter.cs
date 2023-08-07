using System;
using System.Collections;
using DefaultNamespace;
using Enums;
using GameState;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Characters
{
	public class CharacterShooter : MonoBehaviour
	{
		public event Action<Weapon> OnShoot;

		[SerializeField] private Vector3 _offset;
		[SerializeField] private Weapon _weapon;
		public Bullet BulletPrefab;

		private Characters.Character _character;
		private float _timeSinceShot = 0;

		private bool _reloading;
		private float _reloadTime;
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
			_timeSinceShot += Ticker.TickInterval;
		}
		public void Init(Characters.Character character)
		{
			_character = character;

			_weapon = Instantiate(_weapon);
		}
		public Vector3 GetCursorPosition()
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hit))
			{
				return hit.point;
			}
			return Vector3.zero;
		}
		public void Shoot()
		{
			if(_character.IsStunned()) return;
			if (_timeSinceShot < 1/_character.Stats[CS.FireRate] || IsReloading()) return;
			
			var t = transform;
			var offsetX = t.right * _offset.x;
			var offsetY = t.up * _offset.y;
			var offsetZ = t.forward * _offset.z;
			var offsetVector = offsetX + offsetY + offsetZ;
			for (int i = 0; i < _character.Stats[CS.BulletsPerShot]; i++)
			{
				var spread = Random.Range(-_character.Stats[CS.Spread], _character.Stats[CS.Spread]);
				var rotationOffset = Quaternion.Euler(0, spread, 0);

				var bullet = Instantiate(BulletPrefab, t.position + offsetVector, t.rotation * rotationOffset);
				bullet.Init(_character,_character.Stats[CS.Damage], _character.Team);
			}
			
			OnShoot?.Invoke(_weapon);
			_timeSinceShot = 0;
		}

		public float GetReloadTime()
		{
			return _reloadTime;
		}

		public bool IsReloading()
		{
			return _reloading;
		}
	}
}