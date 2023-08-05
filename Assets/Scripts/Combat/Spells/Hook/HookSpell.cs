using Characters;
using Enums;
using Events;
using Events.EventData;
using GameState;
using Stats;
using UnityEngine;

namespace Combat.Spells.Hook
{
	public class HookSpell : BaseSpell, IProjectileHandler
	{
		[SerializeField] private Projectile _projectilePrefab;
		[SerializeField] private MinMaxStatRange _hookRange;
		[SerializeField] private MinMaxStatRange _hookSpeed;
		[SerializeField] private MinMaxStatRange _hookDamage;


		private Projectile _forwardProjectile;
		private Projectile _backwardProjectile;

		private Character _target;
		protected override void PopulateParams()
		{
			base.PopulateParams();
			Params.Add(CS.HookRange, _hookRange);
			Params.Add(CS.HookSpeed, _hookSpeed);
			Params.Add(CS.HookDamage, _hookDamage);
		}

		protected override void OnSpellStart()
		{
			base.OnSpellStart();
			var ownerPosition = Owner.transform.position;
			var direction = GetCursor() - ownerPosition;
			var data = new ProjectileData
			{
				Owner = Owner,
				Range = GetParam(CS.HookRange),
				Velocity = direction.normalized * GetParam(CS.HookSpeed),
				StartPosition = ownerPosition,
				Spell = this,
			};
			_forwardProjectile = GameStateController.ProjectileSystem.LaunchProjectile(_projectilePrefab, data, this);
			_forwardProjectile.AddTargetFilter(EntityFilterer.NotSelfFilter);
			_forwardProjectile.AddTargetFilter(EntityFilterer.CharacterFilter);
		}

		public void OnProjectileHit(Projectile projectile)
		{
			if(projectile != _forwardProjectile) return;

			_target = projectile.Data.Target as Character;
			if(_target == null)
			{
				Destroy(_forwardProjectile);
				return;
			}
			GameEvent<AggroEventData>.Invoke(new AggroEventData
			{
				Target = _target,
				AggroFilter = (character) => character.Team == Owner.Team,
			});
			Destroy(_forwardProjectile);
			var targetToOwner = Owner.transform.position - _target.transform.position;
			_backwardProjectile = GameStateController.ProjectileSystem.LaunchProjectile(_projectilePrefab, new ProjectileData
			{
				Owner = Owner,
				Range = targetToOwner.magnitude,
				Velocity = projectile.Data.Velocity.magnitude * targetToOwner.normalized,
				StartPosition = projectile.transform.position,
				Spell = this,
			}, this);
			_backwardProjectile.AddTargetFilter(EntityFilterer.NoneFilter);
		}

		public void OnProjectileTick(Projectile projectile)
		{
			if(projectile != _backwardProjectile) return;
			if(_target == null)
			{
				Destroy(_backwardProjectile);
				return;
			}
			_target.CharacterMover.SetPosition(projectile.transform.position);

			return;
		}
	}
}