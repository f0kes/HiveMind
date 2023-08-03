namespace Misc
{
	public struct TaskResult
	{
		public bool IsResultSuccess;
		public string Message;
		public static implicit operator bool(TaskResult result)
		{
			return result.IsResultSuccess;
		}
		public static implicit operator TaskResult(bool success)
		{
			return new TaskResult { IsResultSuccess = success };
		}
		public static TaskResult Failure(string message)
		{
			return new TaskResult { IsResultSuccess = false, Message = message };
		}

		public static TaskResult Success => new TaskResult { IsResultSuccess = true };

	}
}