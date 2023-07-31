using System;
using UI;
using UnityEngine;

namespace GameState
{
	public class GameStarter : MonoBehaviour
	{
		[SerializeField] private bool _autoStart = false;
		private void Start()
		{
			if(_autoStart)
			{
				Destroy(gameObject);
				return;
			}
			Ticker.I.Pause();
			TextMessageRenderer.Instance.ShowMessage("Press space to start", 100f);
		}
		private void Update()
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				Ticker.I.Unpause();
				TextMessageRenderer.Instance.HideMessage();
				Destroy(gameObject);
			}
		}
	}
}