namespace ChoirAdminApp.Dtos.Choir
{
	public class SetChoirDto
	{
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public Guid? Director { get; set; }
		public List<Guid>? Chorists { get; set; }
	}
}
