using System.ComponentModel.DataAnnotations;

namespace ChoirAdminApp.Models
{
	public class User
	{
		//	TODO: add 1:1 relationship to user profile
		public Guid Id { get; set; }
		[Required]
		[StringLength(50)]
		public string Username { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
		public Guid RoleId { get; set; }
		public Role Role { get; set; }
		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpirationTime { get; set; }
	}
}
