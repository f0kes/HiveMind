using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
	//create asset menu
	[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
	public class Weapon : ScriptableObject
	{
		public Sprite Icon;
		public List<AudioClip> fireSounds;
		public float Range;
		public float Damage;
		public float FireRate;
		public float Spread;
		public int BulletsPerShot =1;
		
		public int ClipSize;
		public float ReloadTime;

		private int _currentBullets;

		private void Awake()
		{
			_currentBullets = ClipSize;
		}

		public int CurrentBullets
		{
			get => _currentBullets;
			set { _currentBullets = value; }
		}

		public void Shoot()
		{
			CurrentBullets--;
		}
		
	}
}