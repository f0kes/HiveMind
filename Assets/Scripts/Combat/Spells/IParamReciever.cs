using System;

namespace Combat.Spells
{
	public interface IParamReciever<T> where T : Enum
	{
		void SetParamProvider(IParamProvider<T> paramProvider);
	}
}