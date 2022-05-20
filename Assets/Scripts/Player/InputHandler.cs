using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using DefaultNamespace;
using DefaultNamespace.UI;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
	public class InputHandler : CharacterControlsProvider
	{
		//singleton
		public static InputHandler Instance;

		private Camera _mainCamera;
		private bool _inputEnabled = true;

		private Character _mouseOverCharacter;


		private ObjectGizmo _mouseOverObjectGizmo;

		private Vector3 _lookAt;

		private float _savedDesirability;
		
		[SerializeField] private float _desirability = 10;
		
		[SerializeField] private bool _cheatsEnabled;

		protected override void Awake()
		{
			base.Awake();
			if (Instance == null)
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


		private void Start()
		{
			NewCharacter(ControlledCharacter);
			
		}

		private void OnOldCharacterReplaced(Character obj)
		{
			//obj.AIDesirability = _savedDesirability;
			obj.OnDeath -= OnCharacterDeath;
		}

		private void NewCharacter(Character obj)
		{
			//_savedDesirability = obj.AIDesirability;
			//obj.AIDesirability = _desirability;
			obj.OnDeath += OnCharacterDeath;
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

		public Character GetControlledCharacter()
		{
			return ControlledCharacter;
		}
		private void Update()
		{
			if (!_inputEnabled)
				return;
			Vector2 moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

			var (success, position) = GetMousePosition();
			if (success)
			{
				_lookAt = position;
			}

			var (success2, character) = GetMouseOverCharacter();
			if (success2)
			{
				_mouseOverCharacter = character;
				if (character.Team == ControlledCharacter.Team)
				{
					//_objectGizmo.gameObject.SetActive(false);
					_mouseOverObjectGizmo = ObjectGizmo.GetGizmo(_mouseOverCharacter.transform);
					_mouseOverObjectGizmo.gameObject.SetActive(true);
					_mouseOverObjectGizmo.healthBar.SetEntity(_mouseOverCharacter);
					if (character != ControlledCharacter)
						TextMessageRenderer.Instance.ShowActionPrompt("SPC-SWAP");
				}
			}
			else
			{
				TextMessageRenderer.Instance.HideActionPrompt();
				_mouseOverCharacter = null;
				if (_mouseOverObjectGizmo != null)
				{
					_mouseOverObjectGizmo.gameObject.SetActive(false);
				}
			}

			bool shouldShoot = Input.GetMouseButton(0);
			CharacterMover.SetInput(moveDirection, _lookAt);
			if (shouldShoot)
			{
				ControlledCharacter.CharacterShooter.Shoot();
			}

			bool shouldInteract = Input.GetKeyDown(KeyCode.E);
			if (shouldInteract)
			{
				ControlledCharacter.CharacterInteractor.Interact();
			}

			bool shouldSwap = Input.GetKeyDown(KeyCode.Space);
			if (shouldSwap)
			{
				// var entities = EntityList.GetEntitiesOnTeam(ControlledCharacter.Team);
				// var friends = entities.Select(e => e as Prison).Where(c => c != null).ToList();
				// var toSwap = friends.First(x => x != ControlledCharacter);
				if (_mouseOverCharacter != null && _mouseOverCharacter.Team == ControlledCharacter.Team)
				{
					ControlledCharacter.SwapControlsProvider(_mouseOverCharacter);
				}
			}

			bool shouldReload = Input.GetKeyDown(KeyCode.R);
			if (shouldReload)
			{
				ControlledCharacter.CharacterShooter.Reload();
			}

			bool shouldToggleMap = Input.GetKeyDown(KeyCode.Tab);
			if (shouldToggleMap)
			{
				Map.Instance.ToggleMap();
			}

			if (_cheatsEnabled)
			{
				bool shouldGoToNextLevel = Input.GetKeyDown(KeyCode.F1);
				bool shouldDie = Input.GetKeyDown(KeyCode.F2);
				bool shouldGodMode = Input.GetKeyDown(KeyCode.F3);
				bool shouldKillTeam = Input.GetKeyDown(KeyCode.F4);

				if (shouldGoToNextLevel)
				{
					MeshBulilder.I.NextLevel();
				}

				if (shouldDie)
				{
					ControlledCharacter.TakeDamage(10000);
				}

				if (shouldGodMode)
				{
					ControlledCharacter.SetMaxHealth(10000);
				}

				if (shouldKillTeam)
				{
					var entities = EntityList.GetEntitiesOnTeam(ControlledCharacter.Team);
					foreach (var entity in entities)
					{
						if (entity != ControlledCharacter)
						{
							entity.TakeDamage(10000);
						}
					}
				}
			}
		}

		private (bool success, Vector3 position) GetMousePosition()
		{
			var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

			return Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground"))
				? (success: true, position: hitInfo.point)
				: (success: false, position: Vector3.zero);
		}

		private (bool success, Character character) GetMouseOverCharacter()
		{
			var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;

			//spherecast to find character
			bool success = Physics.SphereCast(ray, 1.5f, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Character"));
			//bool success = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Prison"));
			if (!success)
			{
				return (success: false, character: null);
			}

			var character = hitInfo.transform.GetComponent<Character>();
			return character != null ? (success: true, character: character) : (success: false, character: null);
		}
	}
}