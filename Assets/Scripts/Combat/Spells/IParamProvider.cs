using System;
using Enums;

namespace Combat.Spells
{
	public interface IParamProvider<in T> where T : Enum
	{
		bool ContainsParam(T statName);

		float GetParam(T statName);
	}
}