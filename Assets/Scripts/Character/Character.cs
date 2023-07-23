using System.Collections.Generic;
using System.Linq;
using Combat.Spells;
using Combat.Spells.Heal;
using DefaultNamespace;
using GameState;
using UnityEngine;


namespace Characters
{
	[RequireComponent(typeof(CharacterShooter))]
	public class Character : Entity
	{
		public float AIDesirability = 1;
		public float AIThreat = 1;
		
		private Character _swapTarget;


		public CharacterControlsProvider ControlsProvider;
		public CharacterMover CharacterMover{get; private set;}
		public CharacterShooter CharacterShooter{get; private set;}

		[SerializeField] private List<BaseSpell> _spells = new();

		public BaseSpell Spell => _spells.Count != 0 ? _spells[0] : null;


		protected override void ChildAwake()
		{
			gameObject.layer = LayerMask.NameToLayer("Character");
			
			CharacterMover = gameObject.GetComponent<CharacterMover>();
			if(CharacterMover == null)
			{
				CharacterMover = gameObject.AddComponent<CharacterMover>();
			}
			CharacterShooter = gameObject.GetComponent<CharacterShooter>();

			CharacterShooter.Init(this);
		}
		public void InitSpell(BaseSpell spell)
		{
			spell = Instantiate(spell);
			spell.SetOwner(this);
			spell.OnCreated();
			_spells.Add(spell);
		}
		public void InitSpells()
		{
			if(_spells.Count == 0)
			{
				var heal = BasicHealSpell.CreateDefault();
				var attack = BaseSpell.CreateDefault();
				InitSpell(heal);
				InitSpell(attack);
			}
			foreach(var copy in new List<BaseSpell>(_spells.Select(Instantiate)))
			{
				InitSpell(copy);
			}
		}


		protected override void ChildStart()
		{
			base.ChildStart();
			InitSpells();
			SubscribeToEvents();
		}
		private void OnDestroy()
		{
			UnSubscribeFromEvents();
		}
		private void SubscribeToEvents()
		{
			Ticker.OnTick += OnTick;
		}
		private void UnSubscribeFromEvents()
		{
			Ticker.OnTick -= OnTick;
		}
		private void OnTick(Ticker.OnTickEventArgs obj)
		{
		}

		public void CastSpell(int index = 0)
		{
			if(index < _spells.Count)
			{
				_spells[index].Cast();
			}
		}

		public Vector3 GetCursor()
		{
			return CharacterMover.GetCursor();
		}
		public Entity GetCursorTarget()
		{
			return CharacterMover.GetCursorTarget();
		}
	}
}