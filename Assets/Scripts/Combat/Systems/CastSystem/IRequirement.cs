using Misc;

namespace Combat.CastSystem
{
	public interface IRequirement<in T>
	{
		TaskResult DoesSatisfy(T spell);
	}
}