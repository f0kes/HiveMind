using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		transform.position = InputHandler.Instance.GetControlledCharacter().transform.position;
	}
}