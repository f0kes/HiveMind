using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace DefaultNamespace.UI
{
	public class ObjectGizmo : MonoBehaviour
	{
		[SerializeField] private ObjectGizmo _defaultGizmo;
		private static ObjectGizmo _default;
		private Entity _attachedEntity;
		private Vector3 _offset;
		public static ObjectGizmo Default;

		[SerializeField] private HealthBar healthBar;
		private static Dictionary<Transform, ObjectGizmo> Gizmos = new Dictionary<Transform, ObjectGizmo>();

		private void Awake()
		{
			if(Default == null)
			{
				Default = _defaultGizmo;
			}
		}
		public void AttachTo(Entity e)
		{
			healthBar.SetEntity(e);
			_attachedEntity = e;
			_offset = transform.position - e.transform.position;
		}
		private void Update()
		{
			if(_attachedEntity != null)
			{
				transform.position = _attachedEntity.transform.position + _offset;
			}
		}

		public static ObjectGizmo GetGizmo(Transform t)
		{
			if(!Gizmos.ContainsKey(t))
			{
				var g = Instantiate(Default, t, true);
				var position = t.position;
				g.transform.position = new Vector3(position.x, position.y + 8f, position.z);
				Gizmos.Add(t, g);
			}
			Gizmos[t].gameObject.SetActive(true);
			return Gizmos[t];
		}
	}
}