using System.ComponentModel.DataAnnotations;

namespace ChoirAdminApp.Models
{
	public class Role
	{
		public Guid RoleId { get; set; }
		[Required]
		public string RoleName { get; set; } = string.Empty;
	}
}
