using System;
using System.Collections.Generic;
using Characters;
using Cysharp.Threading.Tasks;
using Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class ManaBar : MonoBehaviour
	{
		[SerializeField] private ManaSegment _manaSegmentPrefab;
		[SerializeField] private float _maxScale = 3f;
		[SerializeField] private float _animationTime = 0.2f;
		[SerializeField] private float _animationDelay = 0.1f;
		[SerializeField] private float _glowPower = 2f;
		[SerializeField] private TextMeshProUGUI _spellUsesText;
		private Character _character;

		private List<ManaSegment> _allSegments = new();
		private LinkedList<ManaSegment> _toActivate = new();
		private LinkedList<ManaSegment> _toDeactivate = new();

		private int _previousMana;
		private int _maxMana;

		private bool _isAnimating = true;

		private void OnDestroy()
		{
			_isAnimating = false;
		}
		public void SetCharacter(Character character)
		{
			_character = character;
			if(character.ActiveSpell == null)
			{
				return;
			}
			_maxMana = character.ActiveSpell.ManaCost;
			_character.Events.ManaChanged += OnManaChanged;
			_character.Events.SpellUsesChanged += OnSpellUsesChanged;
			for(int i = 0; i < _maxMana; i++)
			{
				var instance = Instantiate(_manaSegmentPrefab, transform);
				_toActivate.AddLast(instance);
				_allSegments.Add(instance);
				instance.Disable();
			}
			_previousMana = 0;

			OnManaChanged(_character.CurrentMana);
			OnSpellUsesChanged(_character.ActiveSpell.Uses);
		}

		private void OnSpellUsesChanged(int newUses)
		{
			if(_character.ActiveSpell.IsInfinite) return;
			_spellUsesText.text = newUses.ToString();
		}


		private void OnManaChanged(int newMana)
		{
			var diff = newMana - _previousMana;
			if(diff > 0)
			{
				AddMultiple(diff);
			}
			else if(diff < 0)
			{
				RemoveMultiple(-diff);
			}
			_previousMana = newMana;
			CheckGlow(newMana);
		}
		private async void AddMultiple(int count)
		{
			for(int i = 0; i < count; i++)
			{
				while (_toActivate.Count == 0)
				{
					await UniTask.Yield();
				}
				var segment = _toActivate.First.Value;
				_toActivate.RemoveFirst();
				ActivateManaSegment(segment);
				_toDeactivate.AddLast(segment);
				await UniTask.WaitForSeconds(_animationDelay);
			}
		}
		private async void RemoveMultiple(int count)
		{
			for(int i = 0; i < count; i++)
			{
				while (_toDeactivate.Count == 0)
				{
					await UniTask.Yield();
				}
				var segment = _toDeactivate.Last.Value;
				_toDeactivate.RemoveLast();
				DeactivateManaSegment(segment);
				_toActivate.AddFirst(segment);
				await UniTask.WaitForSeconds(_animationDelay);
			}
		}
		private void CheckGlow(int newMana)
		{
			if(newMana == _maxMana)
			{
				foreach(var segment in _allSegments)
				{
					segment.Glow(_glowPower);
				}
			}
			else
			{
				foreach(var segment in _allSegments)
				{
					segment.DisableGlow();
				}
			}
		}
		private async void ActivateManaSegment(ManaSegment segment)
		{
			segment.Enable();
			segment.transform.localScale = new Vector3(_maxScale, _maxScale, _maxScale);
			var time = 0f;
			var initalColor = segment.GetInitialColor();
			while (time < _animationTime && _isAnimating)
			{
				time += Time.deltaTime;
				var blend = Tween.EaseInSine(time / _animationTime);
				var scale = Mathf.Lerp(_maxScale, 1f, blend);
				var color = Color.Lerp(Color.clear, initalColor, blend);
				segment.transform.localScale = new Vector3(scale, scale, scale);
				segment.SetColor(color);
				await UniTask.Yield();
			}
		}
		private async void DeactivateManaSegment(ManaSegment segment)
		{
			var time = 0f;
			var initalColor = segment.GetInitialColor();
			while (time < _animationTime && _isAnimating)
			{
				time += Time.deltaTime;
				var blend = Tween.EaseOutSine(time / _animationTime);
				var scale = Mathf.Lerp(1f, _maxScale, blend);
				var color = Color.Lerp(initalColor, Color.clear, blend);
				segment.transform.localScale = new Vector3(scale, scale, scale);
				segment.SetColor(color);
				await UniTask.Yield();
			}
			segment.Disable();
		}
	}
}