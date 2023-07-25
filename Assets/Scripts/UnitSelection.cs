using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
	[Serializable]
	public struct ColorCode
	{
		public int Team;
		public Color Color;
	}
	[SerializeField] private Characters.Character _character;
	[SerializeField] private List<ColorCode> _colorCodes;
	[SerializeField] private MeshRenderer _meshRenderer;
	private Material _material;
	private Dictionary<int, Color> _colors = new Dictionary<int, Color>();
	private bool _isPlayer;
	private void Awake()
	{
		//copy material
		_material = new Material(_meshRenderer.material);
		_meshRenderer.material = _material;


		foreach(var colorCode in _colorCodes)
		{
			_colors.Add(colorCode.Team, colorCode.Color);
		}
	}
	void Start()
	{
		_material.color = _colors[_character.Team];

		InputHandler.Instance.OnNewCharacter += OnNewCharacter;
		InputHandler.Instance.OnMouseOverCharacter += OnMouseOverCharacter;
		InputHandler.Instance.OnMouseOverCharacterEnd += OnMouseOverCharacterEnd;
	}
	private void OnDestroy()
	{
		if(InputHandler.Instance == null) return;
		InputHandler.Instance.OnNewCharacter -= OnNewCharacter;
		InputHandler.Instance.OnMouseOverCharacter -= OnMouseOverCharacter;
		InputHandler.Instance.OnMouseOverCharacterEnd -= OnMouseOverCharacterEnd;
	}
	private void OnMouseOverCharacterEnd(Characters.Character character)
	{
		if(!_isPlayer)
		{
			_material.color = _colors[_character.Team];
		}
	}
	private void OnMouseOverCharacter(Characters.Character character)
	{
		if(character == _character && !_isPlayer)
		{
			_material.color = Color.white;
		}
	}

	private void OnNewCharacter(Characters.Character obj)
	{
		if(obj == _character)
		{
			_isPlayer = true;
			_material.color = Color.white;
		}
		else
		{
			_isPlayer = false;
			_material.color = _colors[_character.Team];
		}
	}
}