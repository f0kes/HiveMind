namespace Events.Implementations
{
	public struct FatigueEventData
	{
		public readonly int FatigueValue;
		public FatigueEventData(int fatigueValue)
		{
			FatigueValue = fatigueValue;
		}
	}
	public abstract class FatigueEvent : GameEvent<FatigueEventData>
	{

	}
}