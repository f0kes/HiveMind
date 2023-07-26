using System;
using UnityEngine;

namespace GameState
{
	[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
	public class GameData : ScriptableObject
	{
		[Serializable]
		public struct Ratio
		{
			public int Numerator;
			public int Denominator;
			public float Value => (float)Numerator / Denominator;
		}
		public Ratio EnemyToPlayerLevelScaling = new() { Numerator = 4, Denominator = 3 };
		public uint MaxLevel = 1000;
		public uint GoldPerBattle = 10;
		public uint StartingGold = 10;
		public uint StartingBattleLevel = 1;
		public uint StartingShopLevel = 1;
		public float MaxStatValue = 1000f;
		public float SwapCooldown = 3f;
		public int RollCost = 1;
		public int BuyCost = 3;
	}
}