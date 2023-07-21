using System.Collections.Generic;
using AI;
using UnityEngine;

namespace DefaultNamespace.AI
{
	public abstract class PointGetter
	{
		public abstract List<DesirabilityPoint> GetPoints(Characters.Character entity, List<Characters.Character> entities);
	}
}