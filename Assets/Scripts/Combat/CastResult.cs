using Enums;

namespace Combat
{
	public struct CastResult
	{
		public CastResultType ResultType;
		public string Message;
		public CastResult(CastResultType resultType, string message = "")
		{
			ResultType = resultType;
			Message = message;
		}
		public static CastResult Success => new(CastResultType.Success);
		public static CastResult Fail => new(CastResultType.Fail);

		public static CastResult NoTarget => new(CastResultType.Fail, "No target");

		//implicit conversion to bool
		public static implicit operator bool(CastResult result)
		{
			return result.ResultType == CastResultType.Success;
		}
		
	}
}