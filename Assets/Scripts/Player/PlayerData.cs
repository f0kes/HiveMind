using System.Collections.Generic;
using DefaultNamespace;

namespace Player
{
	public class PlayerData
	{
		private int _gold;
		private List<Entity> _party = new List<Entity>();
		private List<Entity> _inventory = new List<Entity>();

		public int Gold
		{
			get => _gold;
			set => _gold = value;
		}
		public void AddToInventory(Entity entity)
		{
			_inventory.Add(entity);
		}
		public void RemoveFromInventory(Entity entity)
		{
			_inventory.Remove(entity);
		}
		
	}
}