namespace ChoirAdminApp.Dtos.Director
{
	public class GetDirectorDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public DateTime Birthdate { get; set; }
		public List<GetRelatedChoirDto> Choirs { get; set; }
	}
}
