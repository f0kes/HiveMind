using Characters;
using Enums;
using GameState;
using Stats;
using UnityEngine;

namespace Combat.Spells.Imp
{
	public class ImpSpell : BaseSpell //todo: generic spawn spell
	{
		[SerializeField] private CharacterData _impData;
		[SerializeField] private MinMaxStatRange _impLevelMultiplier;

		protected override void PopulateParams()
		{
			base.PopulateParams();
			Params.Add(CS.ImpLevel, _impLevelMultiplier);
		}

		public override CastResult CanCastPoint(Vector3 point)
		{
			return CastResult.Success;
		}

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var instance = GameStateController.CharacterFactory.CreateInstant(CharacterData.Copy(_impData), GetOwnerCharacter().Team, GetCursor());
			
			var level = (int)(GetOwnerCharacter().Level * GetParam(CS.ImpLevel));
			if(level < 1) level = 1;
			instance.SetLevel(level);
		}
	}
}