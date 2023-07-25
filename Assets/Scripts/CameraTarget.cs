using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	[SerializeField] private float _threshold = 0.1f;
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
		var character = InputHandler.Instance.GetControlledCharacter();
		if(character == null) return;
		Vector3 playerPos = character.transform.position;
		Vector3 targetPos = (playerPos + _mousePos) / 2;

		targetPos.x = Mathf.Clamp(targetPos.x, -_threshold + playerPos.x, _threshold + playerPos.x);
		targetPos.z = Mathf.Clamp(targetPos.z, -_threshold + playerPos.z, _threshold + playerPos.z);

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