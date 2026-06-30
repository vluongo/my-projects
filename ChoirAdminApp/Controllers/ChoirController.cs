using ChoirAdminApp.Constants;
using ChoirAdminApp.Dtos;
using ChoirAdminApp.Dtos.Choir;
using ChoirAdminApp.Dtos.Director;
using ChoirAdminApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChoirAdminApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChoirController(IChoirService service) : BaseApiController
	{
		[Authorize(Roles = $"{RoleNames.Read}, {RoleNames.Admin}")]
		[HttpGet]
		public async Task<ActionResult<List<GetDirectorDto>>> GetChoirs([FromQuery] QueryParameters parameters) => Ok(await service.GetChoirs(parameters));

		[Authorize(Roles = $"{RoleNames.Read}, {RoleNames.Admin}")]
		[HttpGet("{id}")]
		public async Task<ActionResult<GetChoirDto>> GetChoir(Guid id)
		{
			var choir = await service.GetChoir(id);
			return choir is null ? NotFoundProblem<GetChoirDto>($"Choir with id {id} was not found") : Ok(choir);
		}

		[Authorize(Roles = $"{RoleNames.Create}, {RoleNames.Admin}")]
		[HttpPost]
		public async Task<ActionResult<GetChoirDto>> AddChoir(PostChoirDto choir)
		{
			var createdChoir = await service.AddChoir(choir);
			return CreatedAtAction(nameof(GetChoir), new { id = createdChoir.Id }, createdChoir);
		}

		[Authorize(Roles = $"{RoleNames.Delete}, {RoleNames.Admin}")]
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteChoir(Guid id)
		{
			var deleted = await service.DeleteChoir(id);
			return deleted ? NoContent() : NotFoundProblem("Choir was not found");
		}

		[Authorize(Roles = $"{RoleNames.Edit}, {RoleNames.Admin}")]
		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateChoir(Guid id, PutChoirDto request)
		{
			var deleted = await service.UpdateChoir(id, request);
			return deleted ? NoContent() : NotFoundProblem("Choir was not found");
		}
	}
}
