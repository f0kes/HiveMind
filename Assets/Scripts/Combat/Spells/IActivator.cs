namespace Combat.Spells
{
	public interface IActivator
	{
		void Activate(IActivatable activatable);

		void Deactivate(IActivatable activatable);
	}
}