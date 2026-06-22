using ChoirAdminApp.Dtos;
using ChoirAdminApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChoirAdminApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DirectorController(IDirectorService service) : BaseApiController
	{
		[HttpGet]
		public async Task<ActionResult<List<GetDirectorDto>>> GetDirectors([FromQuery] QueryParameters parameters) => Ok(await service.GetDirectors(parameters));

		[HttpGet("{id}")]
		public async Task<ActionResult<GetDirectorDto>> GetDirector(Guid id) {
			var director = await service.GetDirector(id);
			return director is null ? NotFoundProblem<GetDirectorDto>($"Director with id {id} was not found") : Ok(director);
		}

		[HttpPost]
		public async Task<ActionResult<GetDirectorDto>> AddDirector(PostDirectorDto director)
		{
			var createdDirector = await service.AddDirector(director);
			return CreatedAtAction(nameof(GetDirector), new { id = createdDirector.Id }, createdDirector);
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteDirector(Guid id)
		{
			var deleted = await service.DeleteDirector(id);
			return deleted ? NoContent() : NotFoundProblem("Director was not found");
		}

		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateDirector(Guid id, PutDirectorDto request)
		{
			var deleted = await service.UpdateDirector(id, request);
			return deleted ? NoContent() : NotFoundProblem("Director was not found");
		}
	}
}
