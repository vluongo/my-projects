namespace ChoirAdminApp.Dtos.Choir
{
	public class PutChoirDto
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
		public Guid? Director { get; set; }
		public List<Guid>? Chorists { get; set; }
	}
}
