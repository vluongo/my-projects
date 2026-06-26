using ChoirAdminApp.Dtos.UserRole;

namespace ChoirAdminApp.Services
{
	public interface IUserRoleService
	{
		public Task<GetUserRoleDto> GetUserRoles(Guid id);
		public Task<bool> AddUserRole(PostUserRoleDto request);
		public Task<bool> DeleteUserRole(PostUserRoleDto request);
	}
}
