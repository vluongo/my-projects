using System.ComponentModel.DataAnnotations;

namespace ChoirAdminApp.Models
{
	public class RefreshToken
	{
		public Guid RefreshTokenID { get; set; }
		[Required]
		public string Token { get; set; }
		[Required]
		public DateTime ExpiresAt { get; set; }
		public DateTime? RevokedAt { get; set; }
		public DateTime CreatedAt { get; set; }
		public Guid UserID { get; set; }
		public User User { get; set; }
	}
}
