using System;
using System.Collections;
using DefaultNamespace;
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

		private Character _character;
		private float _timeSinceShot = 0;

		private bool _reloading;
		private float _reloadTime;

		private void Update()
		{
			_timeSinceShot += Time.deltaTime;
		}

		public void Init(Character character)
		{
			_character = character;

			_weapon = Instantiate(_weapon);
		}

		public void Shoot()
		{
			if (_weapon.CurrentBullets <= 0)
			{
				Reload();
				return;
			}

			if (_timeSinceShot < _weapon.FireRate || IsReloading()) return;

			var t = transform;
			var offsetX = t.right * _offset.x;
			var offsetY = t.up * _offset.y;
			var offsetZ = t.forward * _offset.z;
			var offsetVector = offsetX + offsetY + offsetZ;
			for (int i = 0; i < _weapon.BulletsPerShot; i++)
			{
				var spread = Random.Range(-_weapon.Spread, _weapon.Spread);
				var rotationOffset = Quaternion.Euler(0, spread, 0);

				var bullet = Instantiate(BulletPrefab, t.position + offsetVector, t.rotation * rotationOffset);
				bullet.Init(_weapon.Damage, _character.Team);
			}

			_weapon.CurrentBullets--;
			OnShoot?.Invoke(_weapon);
			_timeSinceShot = 0;
		}

		public void Reload()
		{
			if (_reloading || !gameObject.activeInHierarchy) return;
			StartCoroutine(ReloadCoroutine());
		}

		private IEnumerator ReloadCoroutine()
		{
			_reloading = true;
			_reloadTime = _weapon.ReloadTime;
			while (_reloadTime > 0)
			{
				_reloadTime -= Time.deltaTime;
				yield return null;
			}

			_weapon.CurrentBullets = _weapon.ClipSize;
			_reloading = false;
		}

		public int GetAmmo()
		{
			return _weapon.CurrentBullets;
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