using System;
using TMPro;
using UnityEngine;

namespace UI
{
	public class TextMessageRenderer : MonoBehaviour
	{
		public static TextMessageRenderer Instance;

		[SerializeField] private TextMeshProUGUI _messageText;
		[SerializeField] private TextMeshProUGUI _actionPromptText;
		[SerializeField] private RectTransform _blackScreen;
		[SerializeField] private TextMeshProUGUI _blackScreenText;


		private string _defaultBlackScreenText;

		private float _messageMaxTime;
		private float _messageTimer;

		private float _blackScreenMaxTime;
		private float _blackScreenTimer;

		public void Awake()
		{
			if (Instance != null)
			{
				Debug.LogWarning("Multiple instances of TextMessageRenderer found!, Destroying old one");
				Destroy(Instance.gameObject);
			}

			Instance = this;
		}

		private void Start()
		{
			_defaultBlackScreenText = _blackScreenText.text;
			_messageText.text = "";
			_actionPromptText.text = "";
			_blackScreen.gameObject.SetActive(false);
		}

		public void ShowMessage(string message, float duration = 1f)
		{
			_messageText.text = message;
			_messageMaxTime = duration;
			_messageTimer = duration;
		}

		public void ShowBlackScreen(string message, float time = 10f)
		{
			if (message == "")
			{
				message = _defaultBlackScreenText;
			}
			_blackScreenText.text = message;
			ShowBlackScreen(time);
		}

		public void ShowBlackScreen(float time = 10f)
		{
			_blackScreenMaxTime = time;
			_blackScreenTimer = time;
			_blackScreen.gameObject.SetActive(true);
		}

		public void HideBlackScreen()
		{
			_blackScreen.gameObject.SetActive(false);
		}

		public void ShowActionPrompt(string message)
		{
			_actionPromptText.text = message;
		}

		public void HideActionPrompt()
		{
			_actionPromptText.text = "";
		}

		private void Update()
		{
			if (_messageTimer > 0)
			{
				_messageTimer -= Time.deltaTime;
				_messageText.alpha = _messageTimer / _messageMaxTime;
			}
			else
			{
				_messageText.text = "";
			}

			if (_blackScreenTimer > 0)
			{
				_blackScreenTimer -= Time.deltaTime;
			}
			else
			{
				HideBlackScreen();
			}
		}
	}
}