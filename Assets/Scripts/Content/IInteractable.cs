using System;

namespace DefaultNamespace.Content
{
	public interface IInteractable
	{
		public event Action OnInteract;
		public event Action<Characters.Character> OnInteractWho; 
		public void Interact(Characters.Character character);
		
	}
}