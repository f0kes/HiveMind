using System;

namespace Enums
{
	[Flags]
	public enum EntityTag
	{
		Default = 0,
		Character = 1 << 1,
		Projectile = 1 << 2,
	}
}