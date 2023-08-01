using System;
using System.Collections.Generic;
using System.Linq;
using Characters;
using DefaultNamespace;
using DefaultNamespace.UI;
using Events.Implementations;
using GameState;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Player
{
	public class InputHandler : CharacterControlsProvider
	{
		public event Action<Character> OnMouseOverCharacter;
		public event Action<Character> OnMouseOverCharacterEnd;
		public static InputHandler Instance;

		private Inputs _inputs;

		private Camera _mainCamera;
		private bool _inputEnabled = true;

		private Character _mouseOverCharacter;


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

		protected override void Start()
		{
			base.Start();
			Ticker.OnTick += Tick;
		}

		private void OnDestroy()
		{
			if(Instance == this)
			{
				Instance = null;
			}
			OldCharacterReplaced -= OnOldCharacterReplaced;
			OnNewCharacter -= NewCharacter;
			Ticker.OnTick -= Tick;
		}

		private void OnOldCharacterReplaced(Characters.Character obj)
		{
			obj.Events.Death -= OnCharacterDeath;
		}

		private void NewCharacter(Characters.Character obj)
		{
			obj.Events.Death += OnCharacterDeath;
		}
		public void AssignRandomCharacter(uint teamId)
		{
			var team = GameStateController.Battle.EntityRegistry.GetTeam((ushort)teamId);
			var possibleTargets = team.GetCharacters();
			var random = Random.Range(0, possibleTargets.Count);
			var newCharacter = possibleTargets[random];
			if(newCharacter.ControlsProvider != null)
			{
				newCharacter.ControlsProvider.enabled = false;
			}
			SetCharacter(newCharacter);
			CharacterSwappedEvent.Invoke(new CharacterSwappedData(null, newCharacter)); //todo: change, no swap is happening
		}

		private void OnCharacterDeath(Entity character)
		{
			var possibleTargets = character
				.GetTeam()
				.GetCharacters()
				.Where(x => x != character && !x.IsDead)
				.ToList();


			if(possibleTargets.Count == 0) return;

			var random = Random.Range(0, possibleTargets.Count);
			var newCharacter = possibleTargets[random];
			var result = SwapWithNew(newCharacter, true);
			if(result)
				return;
			Debug.LogError("Failed to swap with new character " + result.Message);
		}

		public void DisableInputs()
		{
			_inputEnabled = false;
		}

		public void EnableInputs()
		{
			_inputEnabled = true;
		}

		public Characters.Character GetControlledCharacter()
		{
			return ControlledCharacter;
		}
		private void Update()
		{
			CollectInput();
		}

		private void CollectInput()
		{
			_inputs.Mouse = CollectMouseData();
			_inputs.MouseOverCharacter = CollectMouseOverCharacterData();
			_inputs.Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			_inputs.Shoot = Input.GetMouseButton(0);

			_inputs.Cast |= Input.GetMouseButtonUp(1);
			_inputs.Swap |= Input.GetKeyUp(KeyCode.Space);
			_inputs.Cheats |= Input.GetKeyUp(KeyCode.F1);
		}

		private Inputs.MouseOverCharacterData CollectMouseOverCharacterData()
		{
			var (found, character) = GetMouseOverCharacter();
			var mouseOverCharacterData = new Inputs.MouseOverCharacterData { Found = found, Character = character };
			return mouseOverCharacterData;
		}

		private Inputs.MouseData CollectMouseData()
		{
			var (found, position) = GetMousePosition();
			var mouseData = new Inputs.MouseData { Found = found, Position = position };
			return mouseData;
		}

		private void Tick(Ticker.OnTickEventArgs obj)
		{
			if(!_inputEnabled || ControlledCharacter == null)
				return;

			HandleMouse();
			HandleMove();
			HandleCast();
			HandleShoot();
			HandleSwap();

			if(_cheatsEnabled)
			{
				HandleCheats();
			}

			_inputs.Flush();
		}
		private void HandleSwap()
		{
			var shouldSwap = _inputs.Swap;
			if(shouldSwap)
			{
				if(_mouseOverCharacter != null && _mouseOverCharacter.Team == ControlledCharacter.Team)
				{
					var result = SwapWithNew(_mouseOverCharacter);
					if(!result) TextMessageRenderer.Instance.ShowMessage(result.Message);
				}
			}
		}
		private void HandleShoot()
		{
			bool shouldShoot = _inputs.Shoot;

			if(shouldShoot)
			{
				ControlledCharacter.CharacterShooter.Shoot();
			}
		}
		private void HandleCast()
		{
			bool shouldCast = _inputs.Cast;
			if(ControlledCharacter.ActiveSpell != null && shouldCast)
			{
				var result = ControlledCharacter.ActiveSpell.Cast();
				if(!result)
				{
					TextMessageRenderer.Instance.ShowMessage(result.Message);
				}
				Debug.Log((bool)result + ": " + result.Message);
			}
		}
		private void HandleMouse()
		{
			var (success, position) = (_inputs.Mouse.Found, _inputs.Mouse.Position);
			if(success)
			{
				_lookAt = position;
			}


			var (success2, character) = (_inputs.MouseOverCharacter.Found, _inputs.MouseOverCharacter.Character);
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
		}
		private void HandleMove()
		{
			var moveDirection = _inputs.Move;
			CharacterMover.SetInput(moveDirection, _lookAt);
		}
		private void HandleCheats()
		{
			bool shouldGoToNextLevel = Input.GetKeyDown(KeyCode.F1);

			if(shouldGoToNextLevel)
			{
				MeshBulilder.I.NextLevel();
			}
		}

		private (bool success, Vector3 position) GetMousePosition()
		{
			var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

			return Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground"))
				? (success: true, position: hitInfo.point)
				: (success: false, position: Vector3.zero);
		}

		public (bool success, Character character) GetMouseOverCharacter()
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

			var character = hitInfo.transform.GetComponent<Characters.Character>();


			return character != null ? (success: true, character: character) : (success: false, character: null);
		}
	}
}