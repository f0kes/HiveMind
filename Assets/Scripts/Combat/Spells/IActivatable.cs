namespace Combat.Spells
{
	public interface IActivatable
	{
		float GetLifetime();

		bool IsPermanent();

		void Activate();

		void Deactivate();
	}
}