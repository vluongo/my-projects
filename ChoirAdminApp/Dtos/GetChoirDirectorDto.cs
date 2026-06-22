namespace ChoirAdminApp.Dtos
{
	public class GetChoirDirectorDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime Birthdate { get; set; }
	}
}
