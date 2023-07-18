using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Spells.AutoAttack;
using DefaultNamespace;
using DefaultNamespace.Settings;
using Enums;
using GameState;
using Stats;
using Stats.Structures;
using UnityEngine;

namespace Combat.Spells
{
	[System.Serializable]
	public struct MinMaxStat
	{
		public CS StatName;
		public SpellParam Range;
	}
	[System.Serializable]
	public class SpellParam
	{
		public float Min;
		public float Max;
		public uint Level{get; private set;}
		public float Value => Min + (Max - Min) * Level;
		public SpellParam Reverse => new SpellParam { Min = Max, Max = Min };
		public void SetLevel(uint level)
		{
			Level = level;
		}
	}

	public abstract class BaseSpell : ScriptableObject
	{
		public int Level = 10;
		public Entity Owner{get; private set;}
		[HideInInspector] public Entity Target;

		protected Dictionary<CS, SpellParam> Params = new();
		protected StatDict<CS> Stats;
		public IEnumerable<CS> DependantStats => Params.Keys;


		public SpellBehaviour Behaviour = SpellBehaviour.Default;
		public List<SpellTag> Tags = new();

		public virtual void OnCreated()
		{
			Tags ??= new List<SpellTag>();
			Tags.Add(SpellTag.All);
			PopulateParams();
			Stats = new StatDict<CS>(Level);
			Ticker.OnTick += OnTick;
		}
		public virtual void OnDestroyed()
		{
			Ticker.OnTick -= OnTick;
			Destroy(this);
			UnsubscribeFromEvents();
		}
		public virtual void SetLevel(uint level)
		{
		}

		protected abstract void PopulateParams();

		protected void AddParam(CS paramName, SpellParam param)
		{
			Params[paramName] = param;
		}
		public void SetOwner(Entity owner)
		{
			if(Owner != null)
			{
				OnDetachedFromCharacter();
				UnsubscribeFromEvents();
			}

			Owner = owner;
			SubscribeToEvents();
			OnAttachedToCharacter();
		}
		public Character.Character GetCursorTarget()
		{
			var character = Owner as Character.Character;
			if(character == null)
				return null;
			var target = character.GetCursorTarget() as Character.Character;
			if(target == null)
				Debug.LogError("No target found!!!, check filters");
			return target;
		}
		public Vector3 GetCursor()
		{
			var character = Owner as Character.Character;
			if(character == null)
			{
				Debug.LogError("Owner is not a character");
				return Vector3.zero;
			}
			Vector3 cursor = character.GetCursor();
			return cursor;
		}
		public CastResult Cast()
		{
			var result = CastResult.Success;
			switch(Behaviour)
			{
				case SpellBehaviour.PointTarget:
					var point = GetCursor() == Vector3.zero ? Owner.transform.position : GetCursor();
					result = CanCastPoint(point);
					if(result)
					{
						OnSpellStart();
					}
					break;

				case SpellBehaviour.UnitTarget:
					var target = GetCursorTarget();
					if(target == null)
					{
						return CastResult.NoTarget;
					}
					result = CanCastTarget(target);
					if(result)
					{
						Target = target;
						OnSpellStart();
					}
					break;
				case SpellBehaviour.Default:
					OnSpellStart();
					break;
				case SpellBehaviour.Passive:
					OnSpellStart();
					break;
				default:
					OnSpellStart();
					break;
			}
			return result;
		}
		public virtual CastResult CanCastTarget(Entity target)
		{
			return CastResult.Success;
		}
		public virtual CastResult CanCastPoint(Vector3 point)
		{
			return CastResult.Success;
		}
		public virtual bool ContainsParam(CS statName)
		{
			if(Params == null)
			{
				Debug.LogError("Params is null" + name);
				return false;
			}
			{
				return Params.ContainsKey(statName);
			}
		}
		public virtual float GetParam(CS statName)
		{
			if(!ContainsParam(statName))
			{
				Debug.LogError("Stat " + statName + " is not dependant on this spell");
				return 0;
			}
			var stat = Stats[statName];
			var minMaxStat = Params[statName];

			return stat / GameSettings.MaxStatValue * (minMaxStat.Max - minMaxStat.Min) + minMaxStat.Min;
		}
		private void SubscribeToEvents()
		{
			Owner.Events.BeforeDamageReceived += OnBeforeDamageReceived;
			Owner.Events.AfterDamageReceived += OnAfterDamageReceived;
			Owner.Events.HitReceived += OnHitReceived;
			Owner.Events.BeforeDamageDealt += OnBeforeDamageDealt;
			Owner.Events.AfterDamageDealt += OnAfterDamageDealt;
			Owner.Events.BulletHit += OnBulletHit;
			Owner.Events.HitLanded += OnHitLanded;
		}
		protected virtual void OnAttachedToCharacter()
		{
		}
		private void UnsubscribeFromEvents()
		{
			Owner.Events.BeforeDamageReceived -= OnBeforeDamageReceived;
			Owner.Events.AfterDamageReceived -= OnAfterDamageReceived;
			Owner.Events.HitReceived -= OnHitReceived;
			Owner.Events.BeforeDamageDealt -= OnBeforeDamageDealt;
			Owner.Events.AfterDamageDealt -= OnAfterDamageDealt;
			Owner.Events.BulletHit -= OnBulletHit;
			Owner.Events.HitLanded -= OnHitLanded;
		}
		protected virtual void OnDetachedFromCharacter()
		{
		}
		public Character.Character GetOwnerCharacter()
		{
			return Owner as Character.Character;
		}

		public void ApplySpell(BaseSpell spell, Entity target)
		{
			spell.Owner = Owner;
			spell.Target = target;
			spell.OnCreated();
		}
		public virtual void OnBeforeDamageReceived(Entity attacker, Damage damage)
		{
		}
		public virtual void OnAfterDamageReceived(Entity attacker, Damage damage)
		{
		}
		public virtual void OnHitLanded(Entity target)
		{
		}
		public virtual void OnHitReceived(Entity target)
		{
		}
		public virtual void OnBeforeDamageDealt(Entity target, Damage damage)
		{
		}
		public virtual void OnAfterDamageDealt(Entity target, Damage damage)
		{
		}
		//invoked when spell is casted TODO
		protected virtual void OnSpellStart()
		{
		}
		public virtual void OnBulletHit(Entity target)
		{
		}
		public virtual void OnTick(Ticker.OnTickEventArgs obj)
		{
		}


		public static BaseSpell CreateDefault()
		{
			return CreateInstance<AutoAttackSpell>();
		}
	}
}