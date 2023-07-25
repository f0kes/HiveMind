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
		var character = InputHandler.Instance.GetControlledCharacter();
		if(character == null) return;
		var playerTeam = character.GetTeam();
		if (playerTeam == null) return;
		if(playerTeam.CanSwap())
		{
			TextAsset.text = "R";
		}
		else
		{
			var cooldown = playerTeam.GetSwapCooldown();
			TextAsset.text = cooldown.ToString("F2", CultureInfo.InvariantCulture);
		}
		
		
	}
}