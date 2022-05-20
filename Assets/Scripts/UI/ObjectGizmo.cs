using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.UI
{
	public class ObjectGizmo : MonoBehaviour
	{
		[SerializeField] private ObjectGizmo _defaultGizmo;
		public static ObjectGizmo Default;
		public HealthBar healthBar;
		private static Dictionary<Transform, ObjectGizmo> Gizmos = new Dictionary<Transform, ObjectGizmo>();

		private void Awake()
		{
			gameObject.SetActive(false);
			if (Default == null)
			{
				Default = _defaultGizmo;
			}
		}

		public static ObjectGizmo GetGizmo(Transform t)
		{
			if (!Gizmos.ContainsKey(t))
			{
				var g = Instantiate(Default, t, true);
				var position = t.position;
				g.transform.position = new Vector3(position.x, position.y+8f, position.z);
				Gizmos.Add(t, g);
			}
			Gizmos[t].gameObject.SetActive(true);
			return Gizmos[t];
		}
	}
}