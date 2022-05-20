using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Characters;
using Player;
using TMPro;
using UnityEngine;

public class AmmoText : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI TextAsset;

	void Update()
	{
		CharacterShooter shooter = InputHandler.Instance.GetControlledCharacter().CharacterShooter;
		var ammo = shooter.GetAmmo();
		TextAsset.text = shooter.IsReloading()
			? "R " + shooter.GetReloadTime().ToString("F2").Replace(",", ".")
			: ammo.ToString();
	}
}