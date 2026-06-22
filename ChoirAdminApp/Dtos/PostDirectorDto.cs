namespace ChoirAdminApp.Dtos
{
	public class PostDirectorDto
	{
		public string Name { get; set; }
		public DateTime Birthdate { get; set; }
		public List<Guid>? Choirs { get; set; }
	}
}
