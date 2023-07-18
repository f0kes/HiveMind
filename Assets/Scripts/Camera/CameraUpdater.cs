using System;
using Cinemachine;
using GameState;
using UnityEngine;

namespace CameraScripts
{
	[RequireComponent(typeof(CinemachineBrain))]
	public class CameraUpdater : MonoBehaviour
	{
		private CinemachineBrain _brain;
		private void Awake()
		{
			_brain = GetComponent<CinemachineBrain>();
		}
		private void Start()
		{
			Ticker.OnTick += OnTick;
		}
		private void OnDestroy()
		{
			Ticker.OnTick -= OnTick;
		}

		private void OnTick(Ticker.OnTickEventArgs obj)
		{
			_brain.ManualUpdate();
		}
	}
}