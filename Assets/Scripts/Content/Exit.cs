using DefaultNamespace.Content;
using Player;
using UI;
using UnityEngine;

namespace Content
{
	[RequireComponent(typeof(Interactable))]
	public class Exit :MonoBehaviour
	{
		private Interactable _interactable;
		private void Awake()
		{
			_interactable = GetComponent<Interactable>();
			_interactable.OnInteractWho += Interact;
		}

		private void Interact(Character.Character obj)
		{
			if (obj.ControlsProvider == InputHandler.Instance)
			{
				MeshBulilder.I.NextLevel();
				TextMessageRenderer.Instance.ShowMessage($"LEVEL: {MeshBulilder.I.GetLevel()}", 2f);
			}
		}
	}

}