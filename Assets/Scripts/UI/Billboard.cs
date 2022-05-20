using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
	private void LateUpdate()
	{
		if (Camera.main != null)
		{
			transform.LookAt(transform.position + Camera.main.transform.forward);
		}
	}
}