namespace Misc
{
	public struct TaskResult
	{
		public bool Success;
		public string Message;
		public static implicit operator bool(TaskResult result)
		{
			return result.Success;
		}
		public static implicit operator TaskResult(bool success)
		{
			return new TaskResult { Success = success };
		}
	}
}