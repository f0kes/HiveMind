namespace Combat.Systems.Activator
{
	public interface IActivatable
	{
		float GetLifetime();

		bool IsPermanent();

		void Activate();

		void Deactivate();
	}
}