using System.Collections.Generic;
using System.Linq;
using Characters;
using Combat.Battle;
using Combat.Spells;
using Combat.Systems.Activator;
using Enums;
using GameState;
using Misc;
using UnityEngine;
using VFX;

namespace Combat.CastSystem
{
	public class CastSystem : BattleSystem, ISpellInvoker
	{
		private List<IRequirement<BaseSpell>> _castRequirements = new List<IRequirement<BaseSpell>>();
		private List<IAction<BaseSpell>> _castActions = new List<IAction<BaseSpell>>();
		private IActivator _activator;

		public CastSystem(IBattle battle, IActivator activator) : base(battle)
		{
			_activator = activator;
		}

		public override void SubscribeToEvents()
		{
		}

		public override void UnsubscribeFromEvents()
		{
		}

		public void AddRequirement(IRequirement<BaseSpell> castRequirement)
		{
			_castRequirements.Add(castRequirement);
		}

		public void AddAction(IAction<BaseSpell> castAction)
		{
			_castActions.Add(castAction);
		}

		public void RemoveCastRequirement(IRequirement<BaseSpell> castRequirement)
		{
			_castRequirements.Remove(castRequirement);
		}

		public TaskResult Invoke(BaseSpell spell)
		{
			var result = CanInvoke(spell);
			var character = spell.Owner as Character;

			if(!result) return result;
			if(character == null) return result;

			character.Events.SpellCasted?.Invoke(spell);
			VFXSystem.I.SpawnSpellIcon(spell.Icon, character.transform);

			_activator.Activate(spell);
			spell.Cast();

			foreach(var action in _castActions)
			{
				action.Execute(spell);
			}
			if(!spell.IsInfinite) character.SpendUse();

			return result;
		}

		public TaskResult CanInvoke(BaseSpell spell)
		{
			var requirements = _castRequirements.Select(castRequirement => castRequirement.DoesSatisfy(spell)).ToList();
			foreach(var requirement in requirements.Where(requirement => !requirement))
			{
				return requirement;
			}
			var result = TaskResult.Success;
			var character = spell.Owner as Character; //TODO: make it work for other entities
			if(character == null)
			{
				return new CastResult(CastResultType.Fail, "Owner is not a character");
			}
			if(spell == null)
			{
				return new CastResult(CastResultType.Fail, "Spell is null");
			}
			switch(spell.Behaviour) //todo: multiple behaviours
			{
				case SpellBehaviour.PointTarget:
					var point = spell.GetCursor() == Vector3.zero ? spell.Owner.transform.position : spell.GetCursor();
					result = spell.CanCastPoint(point);
					break;

				case SpellBehaviour.UnitTarget:
					var target = spell.GetCursorTarget();
					if(target == null)
					{
						return TaskResult.Failure("No target found");
					}
					result = spell.CanCastTarget(target);
					if(result)
					{
						spell.SetTarget(target);
					}
					break;
				case SpellBehaviour.NoTarget:
				default:
					break;
			}
			return result;
		}
	}
}