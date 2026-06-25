using System.ComponentModel.DataAnnotations;

namespace ChoirAdminApp.Models
{
	public class User
	{
		public Guid Id { get; set; }
		[Required]
		[StringLength(50)]
		public string Username { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
	}
}
