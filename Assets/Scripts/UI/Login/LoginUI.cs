using System;
using System.Collections;
using System.Collections.Generic;
using GameState;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
	[SerializeField] private Button _okButton;
	[SerializeField] private GameObject _mainPanel;
	[SerializeField] private TMP_InputField _loginInputField;
	private void Start()
	{
		if(GameStateController.Instance.PlayerNameSet)
		{
			_mainPanel.SetActive(false);
			return;
		}
		_mainPanel.SetActive(true);
		_okButton.onClick.AddListener(OnOkClicked);
	}
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Return))
		{
			OnOkClicked();
		}
	}

	private void OnOkClicked()
	{
		var nick = _loginInputField.text;
		if(string.IsNullOrEmpty(nick))
		{
			return;
		}
		GameStateController.Instance.SetPlayerName(nick);
		_mainPanel.SetActive(false);
	}
}