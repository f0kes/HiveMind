﻿using System.Collections.Generic;

namespace Events
{
	public static class EventResetter
	{
		private static List<IResettable> _resettables = new List<IResettable>();
		
		public static  void Add(IResettable resettable)
		{
			_resettables.Add(resettable);
		}
		public static  void Reset()
		{
			foreach(var resettable in _resettables)
			{
				resettable.Reset();
			}
		}
	}
}