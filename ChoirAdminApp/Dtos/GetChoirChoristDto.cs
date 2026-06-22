using ChoirAdminApp.Models;

namespace ChoirAdminApp.Dtos
{
	public class GetChoirChoristDto
	{
		public string Name { get; set; } = string.Empty;
		public DateTime Birthdate { get; set; }
		public ChordEnum Chord { get; set; }
		public string FavouriteFood { get; set; } = string.Empty;
	}
}
