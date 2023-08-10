using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Misc;
using UnityEngine;
using UnityEngine.UI;
using VFX;

public class BuffPopup : VFXEffect
{
	[Serializable]
	public enum PopupType
	{
		Buff,
		Debuff,
		Neutral
	}
	[SerializeField] private Image _greenArrow;
	[SerializeField] private Image _redArrow;
	[SerializeField] private Image _icon;


	[SerializeField] private float _popupDuration;
	[SerializeField] private float _popupDelay = 0.2f;
	[SerializeField] private float _height;
	[SerializeField] private PopupType _type;

	private float _initialHeight;


	private async void Start()
	{
		SetType(_type);
		CopyMaterial();
		await UniTask.Delay((int)(_popupDelay * 1000));
		await PopUp();
		Destroy(gameObject);
	}
	public void SetType(PopupType type)
	{
		_type = type;
		switch(_type)
		{
			case PopupType.Buff:
				_greenArrow.gameObject.SetActive(true);
				_redArrow.gameObject.SetActive(false);
				break;
			case PopupType.Debuff:
				_redArrow.gameObject.SetActive(true);
				_greenArrow.gameObject.SetActive(false);
				break;
			case PopupType.Neutral:
				_redArrow.gameObject.SetActive(false);
				_greenArrow.gameObject.SetActive(false);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	private void CopyMaterial()
	{
		_greenArrow.material = Instantiate(_greenArrow.material);
		_redArrow.material = Instantiate(_redArrow.material);
		_icon.material = Instantiate(_icon.material);
	}
	private async UniTask PopUp()
	{
		var time = 0f;
		while (time < _popupDuration)
		{
			time += Time.deltaTime;
			var blend = Tween.EaseInSine(time / _popupDuration);
			var height = Mathf.Lerp(_initialHeight, _initialHeight + _height, blend);
			var alpha = Mathf.Lerp(1f, 0f, blend);
			var t = transform;
			var localPosition = t.localPosition;
			localPosition = new Vector3(localPosition.x, height, localPosition.z);
			t.localPosition = localPosition;

			_greenArrow.material.color = new Color(1f, 1f, 1f, alpha);
			_redArrow.material.color = new Color(1f, 1f, 1f, alpha);
			_icon.material.color = new Color(1f, 1f, 1f, alpha);

			await UniTask.Yield();
		}
	}

}