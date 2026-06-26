using ChoirAdminApp.Constants;
using ChoirAdminApp.Dtos;
using ChoirAdminApp.Dtos.Director;
using ChoirAdminApp.Dtos.UserRole;
using ChoirAdminApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChoirAdminApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserRoleController(IUserRoleService service) : BaseApiController
	{
		[Authorize(Roles = RoleNames.Admin)]
		[HttpGet("{id}")]
		public async Task<ActionResult<GetUserRoleDto>> GetUserRoles(Guid id) => Ok(await service.GetUserRoles(id));

		[Authorize(Roles = RoleNames.Admin)]
		[HttpPost]
		public async Task<ActionResult> AddUserRole(PostUserRoleDto userRole)
		{
			var created = await service.AddUserRole(userRole);
			return created ? NoContent() : NotFoundProblem("User or role was not found");
		}

		[Authorize(Roles = RoleNames.Admin)]
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteUserRole(PostUserRoleDto userRole)
		{
			var deleted = await service.DeleteUserRole(userRole);
			return deleted ? NoContent() : NotFoundProblem("Role was not found for the user");
		}
	}
}
