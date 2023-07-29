using System.Linq;
using DefaultNamespace;
using Enums;
using Stats;
using UnityEngine;

namespace Combat.Spells.Hellfire
{
	public class HellfireSpell : BaseSpell
	{
		[SerializeField] private MinMaxStatRange _damage;

		protected override void PopulateParams()
		{
			AddParam(CS.HellfireDamage, _damage);
		}

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var damage = GetParam(CS.HellfireDamage);
			var allCharacters = GlobalEntities.GetAllCharacters();
			foreach(var newDamage in from character in allCharacters
			        where !character.IsDead
			        select new Damage
			        {
				        Source = GetOwnerCharacter(),
				        Target = character,
				        Spell = this,
				        Value = damage,
				        Redirecrable = true
			        })
			{
				BattleProcessor.ProcessHit(newDamage);
			}
		}
	}
}