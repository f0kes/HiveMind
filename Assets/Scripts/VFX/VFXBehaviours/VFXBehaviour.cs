namespace VFX
{
	public abstract class VFXBehaviour
	{
		protected VFXEffect Effect;
		public VFXBehaviour(VFXEffect effect)
		{
			Effect = effect;
		}
		public abstract void Execute();
		
	}
}