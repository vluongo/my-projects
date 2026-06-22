using System.ComponentModel.DataAnnotations;

namespace ChoirAdminApp.Models
{
	public class Choir
	{
		public Guid ChoirId { get; set; }
		[Required]
		[StringLength(50)]
		public string Name { get; set; } = string.Empty;

		[StringLength(100)]
		public string Description { get; set; } = string.Empty;
		public Guid? DirectorID { get; set; }
		public Director? Director { get; set; }
		
		[Timestamp] // Concurrency token
		public byte[] RowVersion { get; set; }
		public ICollection<Chorist> Chorists { get; set; } = new List<Chorist>();
	}
}
