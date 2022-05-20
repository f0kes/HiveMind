﻿using System.Collections.Generic;
using System.Linq;
using AI.ComplexBehaviours;
using AI.SteeringBehaviours;
using Characters;
using DefaultNamespace;
using DefaultNamespace.AI;
using UnityEngine;

namespace AI
{
	public class CharacterAi : CharacterControlsProvider
	{
		[SerializeField] private float _viewDistance = 50f;
		[SerializeField] private int _circleDivisions = 16;

		[SerializeField] private float _chaseBehaivourWeight = 0.8f;
		[SerializeField] private float _fleeBehaivourWeight = 1f;
		[SerializeField] private float _avoidWallBehaivourWeight = 1f;
		[SerializeField] private float _alignBehaivourWeight = 0f;

		[SerializeField] private bool _debugDraw;

		private Entity _currentEnemy;

		private Vector3 _lastDirection;

		protected List<ComplexBehaviour> Behaviours = new List<ComplexBehaviour>();

		protected override void Awake()
		{
			base.Awake();
			PopulateBehaviours();
		}

		protected virtual void PopulateBehaviours()
		{
			Behaviours.Add(new ComplexBehaviour
			(new EnemyPointGetter(0.1f),
				new AttractBehaviour(), _chaseBehaivourWeight));
			Behaviours.Add(new ComplexBehaviour
			(new WallGetter(_viewDistance, _circleDivisions),
				new RepellBehaviour(), _avoidWallBehaivourWeight*3));
			Behaviours.Add(new ComplexBehaviour
			(new EnemyPointGetter(3f, true),
				new RepellBehaviour(), _fleeBehaivourWeight));
			Behaviours.Add(new ComplexBehaviour
			(new FriendPointGetter(2f),
				new AttractBehaviour(), _alignBehaivourWeight));
		}


		private void Update()
		{
			var entities = GetEntitiesInRange(_viewDistance).OfType<Character>().ToList();
			_currentEnemy = GetClosestEnemy(entities);

			var desirabiltityMap = new DesirabilityMap(_circleDivisions);

			foreach (var behaviour in Behaviours)
			{
				desirabiltityMap +=
					behaviour.GetDesirabilities(ControlledCharacter, entities, desirabiltityMap.Divisions, _debugDraw);
			}

			desirabiltityMap.Remap();

			var maxValue = desirabiltityMap.DirectionMap.Values.Max();
			var maxDirection = desirabiltityMap.DirectionMap.First(x => x.Value == maxValue).Key;

			if (maxValue <= 0.3f)
			{
				maxDirection = Vector2.zero;
			}


			//draw lines
			foreach (var kv in desirabiltityMap.DirectionMap)
			{
				var position = ControlledCharacter.transform.position;
				var direction = new Vector3(kv.Key.x, 0, kv.Key.y);
				Debug.DrawLine(position, position + (direction * kv.Value * 10),
					kv.Value == maxValue ? Color.red : Color.white);
			}


			Vector3 lookAt = Vector3.zero;
			if (_currentEnemy != null)
			{
				lookAt = _currentEnemy.transform.position;
				ControlledCharacter.CharacterShooter.Shoot();
			}

			ControlledCharacter.CharacterMover.SetInput(maxDirection, lookAt);
		}

		private List<Entity> GetEntitiesInRange(float viewDistance)
		{
			var colliders = Physics.OverlapSphere(ControlledCharacter.transform.position, viewDistance);
			return colliders.Select(col => col.GetComponent<Entity>()).Where(entity => entity != null).ToList();
		}

		private Character GetClosestEnemy(List<Character> visibleEntities)
		{
			return visibleEntities
				.OrderBy(entity => Vector3.Distance(ControlledCharacter.transform.position, entity.transform.position))
				.FirstOrDefault(entity => entity.Team != ControlledCharacter.Team);
		}
	}
}