using System;
using System.Collections.Generic;
using Combat;
using UnityEngine;
using UnityEngine.UI;

//create asset menu
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
	public Sprite Icon;
	public List<AudioClip> fireSounds;

}