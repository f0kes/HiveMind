using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationRandomizer : MonoBehaviour
{
	[SerializeField] private bool x;
	[SerializeField] private bool y;
	[SerializeField] private bool z;
	// Start is called before the first frame update
	void Start()
	{
		var rotation = transform.rotation.eulerAngles;
		if(x)
		{
			rotation.x = Random.Range(0, 360);
		}
		if(y)
		{
			rotation.y = Random.Range(0, 360);
		}
		if(z)
		{
			rotation.z = Random.Range(0, 360);
		}
		transform.rotation = Quaternion.Euler(rotation);
	}

	// Update is called once per frame

}