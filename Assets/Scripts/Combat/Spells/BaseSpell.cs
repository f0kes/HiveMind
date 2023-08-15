using System.Collections.Generic;
using Characters;
using Combat.Spells.AutoAttack;
using Combat.Systems.Activator;
using Combat.Systems.ChargeSystem;
using DefaultNamespace;
using DefaultNamespace.Settings;
using Enums;
using Events;
using GameState;
using Stats;
using UnityEngine;
using VFX;

namespace Combat.Spells
{
	[System.Serializable]
	public struct MinMaxStat
	{
		public CS StatName;
		public MinMaxStatRange Range;
	}


	public abstract class BaseSpell : ScriptableObject, IParamProvider<CS>, IActivatable, IChargable
	{
		public int Level = 10;
		public Entity Owner{get; private set;}
		[HideInInspector] protected Entity Target;
		[SerializeField] private Sprite _icon;
		[SerializeField] private string _name;
		[SerializeField] private MinMaxStatRange _lifetime = new MinMaxStatRange(3f, 9f);
		[SerializeField] private MinMaxStatRange _chargeNeeded = new MinMaxStatRange(1000f, 1000f);
		[SerializeField] private MinMaxStatRange _chargePerSecond = new MinMaxStatRange(300f, 1000f);
		[SerializeField] private bool _permanent = true;
		[TextArea(4, 10)] [SerializeField] private string _description;
		[SerializeField] private int _manaCost;
		[SerializeField] private int _uses = 5;
		[SerializeField] private bool _isInfinite = false;
		[SerializeField] private bool _breaks = true;

		protected Dictionary<CS, MinMaxStatRange> Params = new();
		protected StatDict<CS> Stats;
		public IEnumerable<CS> DependantStats => Params.Keys;


		public SpellBehaviour Behaviour;
		public List<SpellTag> Tags = new();

		public Sprite Icon => _icon;
		public string Name => _name;
		public string Description => _description;
		public int ManaCost => _manaCost;
		public int Uses => _uses;
		public bool IsInfinite => _isInfinite;

		protected bool IsActivated;
		public virtual void SetOwner(Entity owner)
		{
			if(Owner != null)
			{
				OnDetachedFromCharacter();
				OnDeactivated();
			}

			Owner = owner;
			if(IsPermanent())
				GameStateController.ActivatorSystem.Activate(this);
			OnAttachedToCharacter();
		}
		public virtual void OnCreated() //todo: check if owner death unsubscribes, ressurection resubscribes
		{
			Tags ??= new List<SpellTag>();
			Tags.Add(SpellTag.All);
			PopulateParams();
			AddParam(CS.Duration, _lifetime);
			AddParam(CS.ChargeNeeded, _chargeNeeded);
			AddParam(CS.ChargePerSecond, _chargePerSecond);
			Stats = Owner.Stats;

			Owner.Events.Death += OnDeath;
			Owner.Events.Ressurect += OnResurrection; //todo: test
		}

		public virtual void OnDestroyed()
		{
			Debug.Log(name + " destroyed");
			OnDeactivated();
		}
		protected virtual void OnDeath(Entity entity)
		{
			OnDeactivated();
		}
		protected virtual void OnResurrection(Entity entity)
		{
			OnActivated();
		}
		public virtual void SetLevel(uint level)
		{
		}

		protected virtual void PopulateParams()
		{
		}

		protected void AddParam(CS paramName, MinMaxStatRange param)
		{
			Params[paramName] = param;
		}

		public Character GetCursorTarget()
		{
			var character = Owner as Character;
			if(character == null)
				return null;
			var target = character.GetCursorTarget() as Character;
			return target;
		}
		public Vector3 GetCursor()
		{
			var character = Owner as Character;
			if(character == null)
			{
				Debug.LogError("Owner is not a character");
				return Vector3.zero;
			}
			var cursor = character.GetCursor();
			return cursor;
		}
		public void Cast() //todo: move to cast system
		{
			OnSpellStart();
		}
		public virtual CastResult CanCastTarget(Entity target)
		{
			return CastResult.Success;
		}
		public virtual Entity ChooseBestTarget(List<Entity> targets)
		{
			return targets[UnityEngine.Random.Range(0, targets.Count)];
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
		protected List<Character> FilterCharacters(EntityFilter filter)
		{
			var filteredCharacters =
				GameStateController //todo: make a method for this
					.Battle
					.EntityRegistry
					.GetAllCharacters()
					.Filter(Owner, filter);
			return filteredCharacters;
		}
		public virtual float GetParam(CS statName)
		{
			if(!ContainsParam(statName))
			{
				Debug.LogError("Stat " + statName + " is not dependant on this spell");
				return 0;
			}
			var stat = Mathf.Min(Stats[statName], GameSettings.MaxStatValue);
			var minMaxStat = Params[statName];
			var val = stat / GameSettings.MaxStatValue * (minMaxStat.Max - minMaxStat.Min) + minMaxStat.Min;
			return Mathf.Clamp(val, minMaxStat.Min, minMaxStat.Max);
		}

		public virtual float GetLifetime()
		{
			return GetParam(CS.Duration);
		}

		public virtual bool IsPermanent()
		{
			return _permanent;
		}

		public void Activate()
		{
			if(IsActivated) return;
			IsActivated = true;
			OnActivated();
		}
		public void Deactivate()
		{
			if(!IsActivated) return;
			IsActivated = false;
			OnDeactivated();
		}

		protected virtual void OnActivated()
		{
			Ticker.OnTick += OnTick;
			Owner.Events.BeforeDamageReceived += OnBeforeDamageReceived;
			Owner.Events.AfterDamageReceived += OnAfterDamageReceived;
			Owner.Events.HitReceived += OnHitReceived;
			Owner.Events.BeforeDamageDealt += OnBeforeDamageDealt;
			Owner.Events.AfterDamageDealt += OnAfterDamageDealt;
			Owner.Events.BulletHit += OnBulletHit;
			Owner.Events.HitLanded += OnHitLanded;

			if(!IsPermanent())
			{
				Debug.Log("Spawning aura, duration: " + GetLifetime());
				VFXSystem.I.PlayEffectFollow(VFXSystem.Data.Aura, Owner.transform, GetLifetime());
			}
		}
		protected virtual void OnDeactivated()
		{
			Ticker.OnTick -= OnTick;
			Owner.Events.BeforeDamageReceived -= OnBeforeDamageReceived;
			Owner.Events.AfterDamageReceived -= OnAfterDamageReceived;
			Owner.Events.HitReceived -= OnHitReceived;
			Owner.Events.BeforeDamageDealt -= OnBeforeDamageDealt;
			Owner.Events.AfterDamageDealt -= OnAfterDamageDealt;
			Owner.Events.BulletHit -= OnBulletHit;
			Owner.Events.HitLanded -= OnHitLanded;
		}
		protected virtual void OnAttachedToCharacter()
		{
		}

		protected virtual void OnDetachedFromCharacter()
		{
		}
		public Characters.Character GetOwnerCharacter()
		{
			return Owner as Characters.Character;
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

		public float GetMaxCharge()
		{
			return GetParam(CS.ChargeNeeded);
		}

		public float GetChargeGain()
		{
			return GetParam(CS.ChargePerSecond);
		}

		public float GetChargeLoss()
		{
			return GetParam(CS.ChargePerSecond);
		}

		public void SetTarget(Entity target)
		{
			Target = target;
		}

		public virtual bool Breaks()
		{
			return _breaks;
		}
	}
}