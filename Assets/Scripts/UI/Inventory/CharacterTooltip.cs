using Characters;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
	public class CharacterTooltip : MonoBehaviour
	{
		[SerializeField] private Image _icon;
		[SerializeField] private TextMeshProUGUI _name;
		[SerializeField] private TextMeshProUGUI _lvl;
		[SerializeField] private TextMeshProUGUI _hp;
		[SerializeField] private TextMeshProUGUI _dps;
		[SerializeField] private TextMeshProUGUI _spellName;
		[SerializeField] private TextMeshProUGUI _spellDescription;
		[SerializeField] private TextMeshProUGUI _type;

		public Tooltip Tooltip{get; private set;}
		private CharacterData _characterData;
		private void Awake()
		{
			Tooltip = gameObject.AddComponent<Tooltip>();
		}
		public void SetData(CharacterData characterData)
		{
			_characterData = characterData;
			_icon.sprite = characterData.EntityData.Icon;
			_name.text = characterData.EntityData.Name;
			_lvl.text = $"Lvl: {characterData.EntityData.Level}";
			_type.text = characterData.Class.ToString();

			var health = (int)characterData.EntityData.GetStats()[CS.Health];
			var damage = characterData.EntityData.GetStats()[CS.Damage];
			var attackSpeed = characterData.EntityData.GetStats()[CS.FireRate];
			var dps = (int)(damage * attackSpeed);
			_hp.text = $"HP: {health}";
			_dps.text = $"DPS: {dps}";
			_spellName.text = characterData.Spells[0].Name;
			_spellDescription.text = characterData.Spells[0].Description;
		}

	}
}