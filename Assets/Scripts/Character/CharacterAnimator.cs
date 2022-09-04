using System;
using UnityEngine;

namespace Characters
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CharacterMover))]
	public class CharacterAnimator : MonoBehaviour
	{
		private Animator _animator;
		private CharacterMover _characterMover;
		
		private static readonly int MoveX = Animator.StringToHash("MoveX");
		private static readonly int MoveZ = Animator.StringToHash("MoveZ");

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_characterMover = GetComponent<CharacterMover>();
		}

		private void Update()
		{
			Vector3 movement = new Vector3(_characterMover.Movement.x, 0, _characterMover.Movement.y); //world space movement
			float velocityX = Vector3.Dot(transform.right, movement); // rotate movement vector to match character's rotation
			float velocityZ = Vector3.Dot(transform.forward, movement); // rotate movement vector to match character's rotation

			_animator.SetFloat(MoveX, velocityX); // set animator float
			_animator.SetFloat(MoveZ, velocityZ); // set animator float
		}
	}
}