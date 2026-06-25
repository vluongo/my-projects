using ChoirAdminApp.Constants;
using ChoirAdminApp.Dtos;
using ChoirAdminApp.Dtos.Director;
using ChoirAdminApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChoirAdminApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DirectorController(IDirectorService service) : BaseApiController
	{
		[Authorize(Roles = $"{RoleNames.Read}, {RoleNames.Admin}")]
		[HttpGet]
		public async Task<ActionResult<List<GetDirectorDto>>> GetDirectors([FromQuery] QueryParameters parameters) => Ok(await service.GetDirectors(parameters));

		[Authorize(Roles = $"{RoleNames.Read}, {RoleNames.Admin}")]
		[HttpGet("{id}")]
		public async Task<ActionResult<GetDirectorDto>> GetDirector(Guid id) {
			var director = await service.GetDirector(id);
			return director is null ? NotFoundProblem<GetDirectorDto>($"Director with id {id} was not found") : Ok(director);
		}

		[Authorize(Roles = $"{RoleNames.Create}, {RoleNames.Admin}")]
		[HttpPost]
		public async Task<ActionResult<GetDirectorDto>> AddDirector(PostDirectorDto director)
		{
			var createdDirector = await service.AddDirector(director);
			return CreatedAtAction(nameof(GetDirector), new { id = createdDirector.Id }, createdDirector);
		}

		[Authorize(Roles = $"{RoleNames.Delete}, {RoleNames.Admin}")]
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteDirector(Guid id)
		{
			var deleted = await service.DeleteDirector(id);
			return deleted ? NoContent() : NotFoundProblem("Director was not found");
		}

		[Authorize(Roles = $"{RoleNames.Edit}, {RoleNames.Admin}")]
		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateDirector(Guid id, PutDirectorDto request)
		{
			var deleted = await service.UpdateDirector(id, request);
			return deleted ? NoContent() : NotFoundProblem("Director was not found");
		}
	}
}
