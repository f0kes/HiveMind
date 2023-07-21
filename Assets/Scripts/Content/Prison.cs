using System;
using Content;
using UI;
using UnityEngine;

namespace DefaultNamespace.Content
{
	[RequireComponent(typeof(Interactable))]
	public class Prison : MonoBehaviour
	{
		private Characters.Character _prisoner;
		private Interactable _interactable;

		private void Awake()
		{
			_interactable = GetComponent<Interactable>();
			_interactable.OnInteract += Interact;
		}

		private void Start()
		{
			_prisoner = ContentContainer.I.GetRandomPrisoner();
		}

		private void Interact()
		{
			Instantiate(_prisoner, transform.position, Quaternion.identity);
			TextMessageRenderer.Instance.ShowMessage($"Prisoner {_prisoner.name} has been released", 5f);
			Destroy(gameObject);
		}
	}
}