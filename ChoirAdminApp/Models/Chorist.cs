using System.ComponentModel.DataAnnotations;

namespace ChoirAdminApp.Models
{
	public class Chorist
	{
		public Guid ChoristID {  get; set; }
		[Required]
		[StringLength(100)]
		public string Name { get; set; } = string.Empty;
		[Required]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime Birthdate { get; set; }

		[EnumDataType(typeof(ChordEnum))]
		public ChordEnum Chord { get; set; }
		[Required]
		public virtual int ChordId
		{
			get
			{
				return (int)this.Chord;
			}
			set
			{
				Chord = (ChordEnum)value;
			}
		}
		public string FavouriteFood { get; set; } = string.Empty;

		public ICollection<Choir> Choirs { get; set; } = new List<Choir>();
	}
}
