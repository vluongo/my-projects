using ChoirAdminApp.Models;

namespace ChoirAdminApp.Dtos.Chorist
{
	public class PutChoristDto
	{
		public string? Name { get; set; }
		public DateTime? Birthdate { get; set; }
		public ChordEnum? Chord { get; set; }
		public string? FavouriteFood { get; set; }
		public List<Guid>? Choirs { get; set; }
	}
}
