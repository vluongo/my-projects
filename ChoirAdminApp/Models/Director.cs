using System.ComponentModel.DataAnnotations;

namespace ChoirAdminApp.Models
{
	public class Director
	{
		public Guid DirectorId { get; set; }
		[Required(AllowEmptyStrings = false)]
		[StringLength(50)]
		public string Name { get; set; }
		
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime Birthdate { get; set; }

		[Timestamp] // Concurrency token
		public byte[] RowVersion { get; set; }

		public ICollection<Choir> Choirs { get; set; } = new List<Choir>();
	}
}
