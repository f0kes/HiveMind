namespace Combat.Systems.Activator
{
	public interface IActivator
	{
		void Activate(IActivatable activatable);

		void Deactivate(IActivatable activatable);
	}
}