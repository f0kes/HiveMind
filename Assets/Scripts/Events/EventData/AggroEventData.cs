using System;
using Characters;
using DefaultNamespace;

namespace Events.EventData
{
	public struct AggroEventData
	{
		public Entity Target{get; set;}
		public Func<Character, bool> AggroFilter{get; set;}
	}
}