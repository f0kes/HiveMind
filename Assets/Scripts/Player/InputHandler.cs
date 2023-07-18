using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using DefaultNamespace;
using DefaultNamespace.UI;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Player
{
	public class InputHandler : CharacterControlsProvider
	{
		public event Action<Character.Character> OnMouseOverCharacter;
		public event Action<Character.Character> OnMouseOverCharacterEnd;
		public static InputHandler Instance;
		private Camera _mainCamera;
		private bool _inputEnabled = true;

		private Character.Character _mouseOverCharacter;


		private ObjectGizmo _mouseOverObjectGizmo;

		private Vector3 _lookAt;

		private float _savedDesirability;

		//[SerializeField] private float _desirability = 10;

		[SerializeField] private bool _cheatsEnabled;

		protected override void Awake()
		{
			base.Awake();
			if(Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}

			OldCharacterReplaced += OnOldCharacterReplaced;
			OnNewCharacter += NewCharacter;
			_mainCamera = Camera.main;
		}
		private void OnDestroy()
		{
			OldCharacterReplaced -= OnOldCharacterReplaced;
			OnNewCharacter -= NewCharacter;
		}

		private void OnOldCharacterReplaced(Character.Character obj)
		{
			//obj.AIDesirability = _savedDesirability;
			obj.Events.Death -= OnCharacterDeath;
		}

		private void NewCharacter(Character.Character obj)
		{
			//_savedDesirability = obj.AIDesirability;
			//obj.AIDesirability = _desirability;
			obj.Events.Death += OnCharacterDeath;
		}

		private void OnCharacterDeath(Entity character)
		{
			//reload scene
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		public void DisableInputs()
		{
			_inputEnabled = false;
		}

		public void EnableInputs()
		{
			_inputEnabled = true;
		}

		public Character.Character GetControlledCharacter()
		{
			return ControlledCharacter;
		}

		private void Update()
		{
			if(!_inputEnabled)
				return;

			Vector2 moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			var (success, position) = GetMousePosition();
			if(success)
			{
				_lookAt = position;
			}

			var (success2, character) = GetMouseOverCharacter();
			if(success2)
			{
				CharacterMover.SetCursorTarget(character);
				if(_mouseOverCharacter != character)
				{
					OnMouseOverCharacterEnd?.Invoke(_mouseOverCharacter);
					_mouseOverCharacter = character;
					OnMouseOverCharacter?.Invoke(character);
					if(character.Team == ControlledCharacter.Team)
					{
						if(character != ControlledCharacter)
							TextMessageRenderer.Instance.ShowActionPrompt("SPC-SWAP");
					}
				}
			}
			else
			{
				CharacterMover.SetCursorTarget(null);
				TextMessageRenderer.Instance.HideActionPrompt();
				OnMouseOverCharacterEnd?.Invoke(_mouseOverCharacter);
				_mouseOverCharacter = null;
				if(_mouseOverObjectGizmo != null)
				{
					_mouseOverObjectGizmo.gameObject.SetActive(false);
				}
			}

			bool shouldCast = Input.GetMouseButtonDown(1);
			if(shouldCast)
			{
				var result = ControlledCharacter.Spell.Cast();
				if(!result)
				{
					TextMessageRenderer.Instance.ShowMessage(result.Message);
				}
				Debug.Log((bool)result + ": " + result.Message);
			}

			bool shouldShoot = Input.GetMouseButton(0);
			CharacterMover.SetInput(moveDirection, _lookAt);
			if(shouldShoot)
			{
				ControlledCharacter.CharacterShooter.Shoot();
			}

			bool shouldInteract = Input.GetKeyDown(KeyCode.E);
			if(shouldInteract)
			{
				ControlledCharacter.CharacterInteractor.Interact();
			}

			bool shouldSwap = Input.GetKeyDown(KeyCode.Space);
			if(shouldSwap)
			{
				// var entities = EntityList.GetEntitiesOnTeam(ControlledCharacter.Team);
				// var friends = entities.Select(e => e as Prison).Where(c => c != null).ToList();
				// var toSwap = friends.First(x => x != ControlledCharacter);
				if(_mouseOverCharacter != null && _mouseOverCharacter.Team == ControlledCharacter.Team)
				{
					SwapWithNew(_mouseOverCharacter);
				}
			}


			bool shouldToggleMap = Input.GetKeyDown(KeyCode.Tab);
			if(shouldToggleMap)
			{
				Map.Instance.ToggleMap();
			}

			if(_cheatsEnabled)
			{
				HandleCheats();
			}
		}
		private void HandleCheats()
		{
			bool shouldGoToNextLevel = Input.GetKeyDown(KeyCode.F1);
			bool shouldDie = Input.GetKeyDown(KeyCode.F2);
			bool shouldGodMode = Input.GetKeyDown(KeyCode.F3);
			bool shouldKillTeam = Input.GetKeyDown(KeyCode.F4);
			bool shouldKillRandom = Input.GetKeyDown(KeyCode.F5);

			if(shouldGoToNextLevel)
			{
				MeshBulilder.I.NextLevel();
			}

			if(shouldDie)
			{
				ControlledCharacter.TakeDamage(10000);
			}

			if(shouldGodMode)
			{
				ControlledCharacter.SetMaxHealth(10000);
			}

			if(shouldKillTeam)
			{
				var entities = EntityList.GetEntitiesOnTeam(ControlledCharacter.Team);
				foreach(var entity in entities)
				{
					if(entity != ControlledCharacter)
					{
						entity.TakeDamage(10000);
					}
				}
			}

			if(shouldKillRandom)
			{
				var entities = EntityList.GetEntitiesOnTeam(ControlledCharacter.Team)
					.Where(e => e != ControlledCharacter).ToList();
				//choose random entity
				var randomEntity = entities[Random.Range(0, entities.Count)];
				randomEntity.TakeDamage(10000);
			}
		}

		private (bool success, Vector3 position) GetMousePosition()
		{
			var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

			return Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground"))
				? (success: true, position: hitInfo.point)
				: (success: false, position: Vector3.zero);
		}

		public (bool success, Character.Character character) GetMouseOverCharacter()
		{
			var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			var toGround = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground"));
			if(!toGround) return (success: false, character: null);
			var groundPoint = hitInfo.point;

			var toCharacterRay = new Ray(groundPoint + Vector3.up * 100, Vector3.down);
			var success = Physics.SphereCast(toCharacterRay, 3f, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Character"));

			if(!success)
			{
				return (success: false, character: null);
			}

			var character = hitInfo.transform.GetComponent<Character.Character>();


			return character != null ? (success: true, character: character) : (success: false, character: null);
		}
	}
}