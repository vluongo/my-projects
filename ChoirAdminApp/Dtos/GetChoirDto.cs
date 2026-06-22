namespace ChoirAdminApp.Dtos
{
	public class GetChoirDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public GetChoirDirectorDto Director { get; set; }
		public List<GetChoirChoristDto> Chorists { get; set; }
	}
}
