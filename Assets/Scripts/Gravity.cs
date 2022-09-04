using System.Collections.Generic;
using UnityEngine;

public class SC_RigidbodyMagnet : MonoBehaviour
{
	public float magnetForce = 100;

	List<Rigidbody> caughtRigidbodies = new List<Rigidbody>();

	void FixedUpdate()
	{
		for (int i = 0; i < caughtRigidbodies.Count; i++)
		{
			var sqrDist = (caughtRigidbodies[i].transform.position - transform.position).sqrMagnitude;
			var force = magnetForce / sqrDist;
			caughtRigidbodies[i].AddForce((caughtRigidbodies[i].transform.position - transform.position).normalized * force);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Rigidbody>())
		{
			Rigidbody r = other.GetComponent<Rigidbody>();

			if (!caughtRigidbodies.Contains(r))
			{
				//Add Rigidbody
				caughtRigidbodies.Add(r);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<Rigidbody>())
		{
			Rigidbody r = other.GetComponent<Rigidbody>();

			if (caughtRigidbodies.Contains(r))
			{
				//Remove Rigidbody
				caughtRigidbodies.Remove(r);
			}
		}
	}
}