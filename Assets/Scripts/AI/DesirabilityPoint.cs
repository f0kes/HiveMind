using UnityEngine;

namespace AI
{
	public struct DesirabilityPoint
	{
		public Vector3 Point;
		public float Desirability;
		
		public DesirabilityPoint(Vector3 point, float desirability)
		{
			Point = point;
			Desirability = desirability;
		}

		public static explicit operator Vector3(DesirabilityPoint dp)
		{
			return dp.Point;
		}

		public static implicit operator DesirabilityPoint(Vector3 v)
		{
			return new DesirabilityPoint() {Point = v, Desirability = 1f};
		}
	}
}