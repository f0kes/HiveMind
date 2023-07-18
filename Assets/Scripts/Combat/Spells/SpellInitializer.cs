using System.Collections.Generic;
using UnityEngine;

namespace Combat.Spells
{
	public class SpellInitializer : MonoBehaviour
	{
		[SerializeField] private List<BaseSpell> _spellPool;
		public static SpellList SpellPool;

		private void Awake()
		{
			Init();
		}
		public void Init()
		{
			foreach(var spell in _spellPool)
			{
				spell.OnCreated();
				SpellPool.Add(spell);
			}
		}
	}
}