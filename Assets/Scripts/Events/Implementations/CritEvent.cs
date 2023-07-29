using Combat;

namespace Events.Implementations
{
	public class CritEvent : GameEvent<CritData>
	{

	}
	public struct CritData
	{
		public Damage Damage;
		public float CritChance;
		public float CritDamage;
	}
}