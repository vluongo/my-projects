using ChoirAdminApp.Data;
using ChoirAdminApp.Dtos.UserRole;
using ChoirAdminApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChoirAdminApp.Services
{
	public class UserRoleService(AppDbContext context, ILogger<DirectorService> logger) : IUserRoleService
	{
		public async Task<bool> AddUserRole(PostUserRoleDto request)
		{
			bool userExists = await context.Users.AnyAsync(u => u.Id == request.UserID);
			bool roleExists = await context.Roles.AnyAsync(r => r.RoleId == request.RoleID);
			if (!userExists || !roleExists)
			{
				logger.LogWarning("Add attempt failed: User or Role not found for UserId={UserId}, RoleId={RoleId}", request.UserID, request.RoleID);
				return false;
			}

			var newUserRole = new UserRole {
				UserID = request.UserID,
				RoleID = request.RoleID,
			};

			context.UserRoles.Add(newUserRole);
			await context.SaveChangesAsync();

			logger.LogInformation("Add UserRole record for UserId={UserId}, RoleId={RoleId}", request.UserID, request.RoleID);

			return true;
		}

		public async Task<bool> DeleteUserRole(PostUserRoleDto request)
		{
			UserRole? userRole = await context.UserRoles.FirstOrDefaultAsync(ur => ur.UserID == request.UserID && ur.RoleID == request.RoleID);

			if (userRole is null)
			{
				logger.LogWarning("Delete attempt failed: UserRole not found for UserId={UserId}, RoleId={RoleId}",request.UserID, request.RoleID);
				return false;
			}
			
			context.UserRoles.Remove(userRole);
			await context.SaveChangesAsync();

			logger.LogInformation("Deleted UserRole record for UserId={UserId}, RoleId={RoleId}",request.UserID, request.RoleID);

			return true;
		}

		public async Task<GetUserRoleDto> GetUserRoles(Guid id)
		{
			List<UserRole> userRoles = await context.UserRoles.Where(ur => ur.UserID == id).Include(ur => ur.Role).ToListAsync();
			List<GetRoleDto> rolesDto = userRoles.Select(ur => new GetRoleDto { Id = ur.RoleID, Name = ur.Role.RoleName }).ToList();
			
			logger.LogInformation("Retrieved roles for UserId={UserId}", id);
			return new GetUserRoleDto { Roles = rolesDto };
		}
	}
}
