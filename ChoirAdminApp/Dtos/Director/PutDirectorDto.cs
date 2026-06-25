namespace ChoirAdminApp.Dtos.Director
{
	public class PutDirectorDto
	{
		public string? Name { get; set; } = string.Empty;
		public DateTime? Birthdate { get; set; }
		public List<Guid>? Choirs { get; set; }
	}
}
