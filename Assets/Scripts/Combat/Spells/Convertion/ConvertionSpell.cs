using Enums;
using Events.Implementations;
using GameState;
using Stats;
using UnityEngine;

namespace Combat.Spells.Convertion
{
	public class ConvertionSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _convertionDamageMultiplier;
		[SerializeField] private MinMaxStatRange _healValue;

		protected override void PopulateParams()
		{
			base.PopulateParams();
			Params.Add(CS.ConvertionDamageMultiplier, _convertionDamageMultiplier);
			Params.Add(CS.ConvertionHealValue, _healValue);
		}

		protected override void OnActivated()
		{
			base.OnActivated();
			HealEvent.Subscribe(OnHeal);
		}

		protected override void OnDeactivated()
		{
			base.OnDeactivated();
			HealEvent.Unsubscribe(OnHeal);
		}

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var healValue = GetParam(CS.ConvertionHealValue);
			var healData = new Combat.Heal
			{
				Source = Owner,
				Target = Owner,
				Value = healValue,
			};
			BattleProcessor.ProcessHeal(Owner, Owner, healData);
		}

		private void OnHeal(Combat.Heal data)
		{
			if(data.Target != GetOwnerCharacter()) return;
			var damage = data.Value * GetParam(CS.ConvertionDamageMultiplier);
			var filter = EntityFilterer.EnemyFilter.And(EntityFilterer.NotDeadFilter);
			var enemies =
				GameStateController
					.Battle
					.EntityRegistry
					.GetAllCharacters()
					.Filter(Owner, filter);
			Debug.Log($"Found {enemies.Count} enemies");
			damage /= enemies.Count;

			foreach(var enemy in enemies)
			{
				var damageData = new Damage
				{
					Redirecrable = true,
					Source = Owner,
					Spell = this,
					Target = enemy,
					Value = damage,
				};
				BattleProcessor.ProcessHit(damageData);
			}
			Debug.Log($"Converted {data.Value} to {damage}");
		}
	}
}