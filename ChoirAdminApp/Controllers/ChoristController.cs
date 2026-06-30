using ChoirAdminApp.Constants;
using ChoirAdminApp.Dtos;
using ChoirAdminApp.Dtos.Chorist;
using ChoirAdminApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChoirAdminApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChoristController(IChoristService service) : BaseApiController
	{
		[Authorize(Roles = $"{RoleNames.Read}, {RoleNames.Admin}")]
		[HttpGet]
		public async Task<ActionResult<List<GetChoristDto>>> GetChoirs([FromQuery] QueryParameters parameters) => Ok(await service.GetChorists(parameters));

		[Authorize(Roles = $"{RoleNames.Read}, {RoleNames.Admin}")]
		[HttpGet("{id}")]
		public async Task<ActionResult<GetChoristDto>> GetChorist(Guid id)
		{
			var chorist = await service.GetChorist(id);
			return chorist is null ? NotFoundProblem<GetChoristDto>($"Chorist with id {id} was not found") : Ok(chorist);
		}

		[Authorize(Roles = $"{RoleNames.Create}, {RoleNames.Admin}")]
		[HttpPost]
		public async Task<ActionResult<GetChoristDto>> AddChorist(PostChoristDto choir)
		{
			var createdChorist = await service.AddChorist(choir);
			return CreatedAtAction(nameof(GetChorist), new { id = createdChorist.Id }, createdChorist);
		}

		[Authorize(Roles = $"{RoleNames.Delete}, {RoleNames.Admin}")]
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteChoir(Guid id)
		{
			var deleted = await service.DeleteChorist(id);
			return deleted ? NoContent() : NotFoundProblem("Chorist was not found");
		}

		[Authorize(Roles = $"{RoleNames.Edit}, {RoleNames.Admin}")]
		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateChoir(Guid id, PutChoristDto request)
		{
			var deleted = await service.UpdateChorist(id, request);
			return deleted ? NoContent() : NotFoundProblem("Chorist was not found");
		}
	}
}
