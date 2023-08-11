using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	[SerializeField] private float _threshold = 0.1f;
	[SerializeField] private float _lerpStep = 0.7f;
	[SerializeField] private bool _followPlayer = true;
	[SerializeField] private Transform _anchorTarget;
	private Camera _mainCamera;
	private Vector3 _mousePos;

	// Start is called before the first frame update
	void Start()
	{
		_mainCamera = Camera.main;
	}

	// Update is called once per frame
	void Update()
	{
		var (success, position) = GetMousePosition();
		if(success)
			_mousePos = position;
		var anchor = _anchorTarget;
		if(_followPlayer)
		{
			var character = InputHandler.Instance.GetControlledCharacter();
			if(character == null) return;
			anchor = character.transform;
		}
		var anchorPosition = anchor.position;
		var targetPos = Vector3.Lerp(anchorPosition, _mousePos, _lerpStep);

		targetPos.x = Mathf.Clamp(targetPos.x, -_threshold + anchorPosition.x, _threshold + anchorPosition.x);
		targetPos.z = Mathf.Clamp(targetPos.z, -_threshold + anchorPosition.z, _threshold + anchorPosition.z);

		transform.position = targetPos;
	}

	private (bool success, Vector3 position) GetMousePosition()
	{
		var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

		return Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground"))
			? (success: true, position: hitInfo.point)
			: (success: false, position: Vector3.zero);
	}
}