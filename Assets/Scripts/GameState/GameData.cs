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
			public float ReverseValue => (float)Denominator / Numerator;
		}
		[Header("Scaling")]
		public Ratio EnemyToPlayerLevelScaling = new() { Numerator = 4, Denominator = 3 };
		public uint MaxLevel = 1000;
		public uint StartingBattleLevel = 1;
		public uint StartingShopLevel = 1;
		public int LevelsPerBattle = 2;

		[Header("Shop")]
		public uint GoldPerBattle = 10;
		public uint StartingGold = 10;
		public int RollCost = 1;
		public int BuyCost = 3;
		public int MaxGold = 26;
		public int ShopRepeats = 8;

		[Header("Battle")]
		public float MaxStatValue = 1000f;
		public float SwapCooldown = 3f;
		public int ManaPerSwap = 1;

		[Header("Fatigue")]
		public float TimeToStartFatigue = 30f;
		public float FatigueTickTime = 3f;
		public int FatigueIncrement = 1;
		public int FatigueStartValue = 1;




	}
}