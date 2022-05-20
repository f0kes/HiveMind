using System;

namespace DefaultNamespace.Content
{
	public interface IInteractable
	{
		public event Action OnInteract;
		public event Action<Character> OnInteractWho; 
		public void Interact(Character character);
		
	}
}