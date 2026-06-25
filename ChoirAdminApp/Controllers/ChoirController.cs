using ChoirAdminApp.Dtos.Choir;
using ChoirAdminApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChoirAdminApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChoirController(IChoirService service) : ControllerBase
	{
		[HttpGet("{id}")]
		public async Task<ActionResult<GetChoirDto>> GetChoir(Guid id)
		{
			var choir = await service.GetChoir(id);
			return choir is null ? NotFound($"Choir with id {id} was not found") : Ok(choir);
		}

		[HttpPost]
		public async Task<ActionResult<GetChoirDto>> AddChoir(SetChoirDto choir)
		{
			var createdChoir = await service.AddChoir(choir);
			return CreatedAtAction(nameof(GetChoir), new { id = createdChoir.Id }, createdChoir);
		}
	}
}
