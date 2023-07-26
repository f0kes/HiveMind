using System.Collections.Generic;

namespace Events
{
	public class EventResetter
	{
		private List<IResettable> _resettables;
		public EventResetter()
		{
			_resettables = new List<IResettable>();
		}
		public void Add(IResettable resettable)
		{
			_resettables.Add(resettable);
		}
		public void Reset()
		{
			foreach(var resettable in _resettables)
			{
				resettable.Reset();
			}
		}
	}
}