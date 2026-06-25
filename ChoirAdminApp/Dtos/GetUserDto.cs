namespace ChoirAdminApp.Dtos
{
	public class GetUserDto
	{
		public Guid Id { get; set; }
		public string Username { get; set; } = string.Empty;
	}
}
