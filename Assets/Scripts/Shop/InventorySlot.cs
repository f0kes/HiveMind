using Characters;

namespace Shop
{
	public class InventorySlot
	{
		private CharacterData _content;
		public CharacterData GetContent()
		{
			return _content;
		}
		public void PutContent(CharacterData character)
		{
			_content = character;
		}
		public void MoveContentTo(InventorySlot other)
		{
			if(other == null || other == this) return;
			other.PutContent(_content);
			_content = null;
		}
	}
}