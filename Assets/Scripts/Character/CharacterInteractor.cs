using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Content;
using UI;
using UnityEngine;

namespace Characters
{
	public class CharacterInteractor : MonoBehaviour
	{
		[SerializeField] private float _interactionDistance = 1.5f;
		private Interactable _closestInteractable;
		private Characters.Character _character;
		public void Init(Characters.Character character)
		{
			_character = character;
		}
		private void Update()
		{
			// overlap sphere to find interactable objects
			var colliders = Physics.OverlapSphere(transform.position, _interactionDistance);
			var interactables = colliders.Select(col => col.GetComponent<Interactable>())
				.Where(interactable => interactable != null).ToList();
			_closestInteractable = interactables
				.OrderBy(interactable => Vector3.Distance(transform.position, interactable.transform.position))
				.FirstOrDefault();
			if (_closestInteractable != null)
			{
				TextMessageRenderer.Instance.ShowActionPrompt("E-Interact");
			}
		}
		public void Interact()
		{
			Debug.Log(_closestInteractable);
			if (_closestInteractable != null)
			{
				_closestInteractable.Interact(_character);
			}
		}
	}
}