namespace Combat.CastSystem
{
	public interface IAction<in T>
	{
		void Execute(T spell);
	}
}