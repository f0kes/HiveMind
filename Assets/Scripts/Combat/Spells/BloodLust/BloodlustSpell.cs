using System.Collections.Generic;
using Characters;
using DefaultNamespace;
using Enums;
using Events.Implementations;
using Stats;
using Stats.Modifiers;
using UnityEngine;

namespace Combat.Spells.BloodLust
{
	public class BloodlustSpell : BaseAura
	{
		[SerializeField] private MinMaxStatRange _levelsOnCrit;
		[SerializeField] private MinMaxStatRange _critChance;
		[SerializeField] private MinMaxStatRange _critDamageMultiplier;

		private StatModifierAdd _critChanceModifier;
		private StatModifierAdd _critDamageModifier;

		protected override void PopulateParams()
		{
			base.PopulateParams();
			AddParam(CS.BloodlustLevelsOnCrit, _levelsOnCrit);
			AddParam(CS.BloodlustCritChance, _critChance);
			AddParam(CS.BloodlustCritDamageMultiplier, _critDamageMultiplier);
		}
		public override void OnCreated()
		{
			base.OnCreated();
			_critChanceModifier = CreateInstance<StatModifierAdd>();
			_critDamageModifier = CreateInstance<StatModifierAdd>();

			_critChanceModifier.SetValFunc(() => GetParam(CS.BloodlustCritChance));
			_critDamageModifier.SetValFunc(() => GetParam(CS.BloodlustCritDamageMultiplier));
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			CritEvent.Subscribe(OnCrit);
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			CritEvent.Unsubscribe(OnCrit);
		}

		private void OnCrit(CritData data)
		{
			var ownerCharacter = GetOwnerCharacter();
			if(ownerCharacter == null) return;
			var damageSource = data.Damage.Source;
			if(damageSource == null) return;
			var levels = GetParam(CS.BloodlustLevelsOnCrit);
			var team = ownerCharacter.GetTeam();
			foreach(var t in team)
			{
				var character = t as Character;
				if(character == null) continue;
				if(character.Class != CharacterClass.Rogue) continue;
				character.LevelUp((int)levels);
			}
		}

		protected override HashSet<Entity> GetTargets()
		{
			var ownerCharacter = GetOwnerCharacter();
			var targets = new HashSet<Entity>();
			if(ownerCharacter == null) return targets;
			var team = ownerCharacter.GetTeam();
			foreach(var t in team)
			{
				targets.Add(t);
			}
			return targets;
		}

		protected override void ApplyAura(Entity target)
		{
			target.Stats.GetStat(CS.CritChance).AddMod(_critChanceModifier);
			target.Stats.GetStat(CS.CritDamage).AddMod(_critDamageModifier);
		}

		protected override void RemoveAura(Entity target)
		{
			target.Stats.GetStat(CS.CritChance).RemoveMod(_critChanceModifier);
			target.Stats.GetStat(CS.CritDamage).RemoveMod(_critDamageModifier);
		}


	}
}