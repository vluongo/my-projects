namespace ChoirAdminApp.Exceptions
{
	public class ChoirAlreadyHasDirectorException : Exception
	{
		public ChoirAlreadyHasDirectorException(string message) : base(message) { }
	}

	public class DefaultRoleNotFoundException : Exception
	{
		public DefaultRoleNotFoundException(string message) : base(message) { }
	}

	public class DirectorNotFoundException : Exception
	{
		public DirectorNotFoundException(string message) : base(message) { }
	}
}
