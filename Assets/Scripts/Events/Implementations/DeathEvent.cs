using DefaultNamespace;

namespace Events.Implementations
{
	public struct DeathData
	{
		public Entity Source;
		public Entity Target;
	}
	public abstract class DeathEvent : GameEvent<DeathData>
	{

	}
}