using ChoirAdminApp.Models;

namespace ChoirAdminApp.Dtos.Chorist;

public class GetChoristDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public DateTime Birthdate { get; set; }
	public ChordEnum Chord { get; set; }
	public string? FavouriteFood { get; set; } = string.Empty;
	public List<GetRelatedChoirDto> Choirs { get; set; }
}
