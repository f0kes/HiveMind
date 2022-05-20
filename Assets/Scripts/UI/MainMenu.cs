using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI _blinkText;
	[SerializeField] private float _blinkSpeed = 0.5f;
	private float _blinkTimer = 0;

	void Start()
	{
		_blinkTimer = _blinkSpeed;
	}


	// Update is called once per frame
	void Update()
	{
		_blinkTimer -= Time.deltaTime;
		if (_blinkTimer <= 0)
		{
			_blinkTimer = _blinkSpeed;
			ToggleBlinkText();
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			SceneManager.LoadScene(1);
		}
	}

	private void ToggleBlinkText()
	{
		_blinkText.enabled = !_blinkText.enabled;
	}
}