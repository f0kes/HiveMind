namespace Combat
{
	public class AmmoContainer
	{
		public float RedAmmo{get; private set;}
		public float BlueAmmo{get; private set;}
		public float GreenAmmo{get; private set;}

		public AmmoContainer(float red, float blue, float green)
		{
			RedAmmo = red;
			BlueAmmo = blue;
			GreenAmmo = green;
		}

		#region Spends

		public bool SpendAmmo(float red, float blue, float green)
		{
			if(RedAmmo < red || BlueAmmo < blue || GreenAmmo < green)
			{
				return false;
			}
			RedAmmo -= red;
			BlueAmmo -= blue;
			GreenAmmo -= green;
			return true;
		}
		public bool SpendRedAmmo(float amount)
		{
			if(RedAmmo < amount)
			{
				return false;
			}
			RedAmmo -= amount;
			return true;
		}
		public bool SpendBlueAmmo(float amount)
		{
			if(BlueAmmo < amount)
			{
				return false;
			}
			BlueAmmo -= amount;
			return true;
		}
		public bool SpendGreenAmmo(float amount)
		{
			if(GreenAmmo < amount)
			{
				return false;
			}
			GreenAmmo -= amount;
			return true;
		}

		#endregion

		#region Adds

		public void AddAmmo(float red, float blue, float green)
		{
			RedAmmo += red;
			BlueAmmo += blue;
			GreenAmmo += green;
		}
		public void AddRedAmmo(float amount)
		{
			RedAmmo += amount;
		}
		public void AddBlueAmmo(float amount)
		{
			BlueAmmo += amount;
		}
		public void AddGreenAmmo(float amount)
		{
			GreenAmmo += amount;
		}

		#endregion
	}
}