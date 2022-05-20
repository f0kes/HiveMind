using System;
using UnityEngine;

namespace DefaultNamespace.Content
{
	public  class Interactable : MonoBehaviour, IInteractable
	{
		public event Action OnInteract;
		public event Action<Character> OnInteractWho;

		public void Interact(Character c)
		{
			OnInteract?.Invoke();
			OnInteractWho?.Invoke(c);
		}
	}
}