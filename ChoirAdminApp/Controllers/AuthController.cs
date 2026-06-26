using ChoirAdminApp.Dtos;
using ChoirAdminApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChoirAdminApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController(IAuthService authService) : BaseApiController
	{
		[HttpPost("register")]
		public async Task<ActionResult<GetUserDto>> Register(PostUserDto request)
		{
			var user = await authService.RegisterAsync(request);
			if (user is null)
			{
				return BadRequestProblem("Username already exists.");
			}

			return Ok(user);
		}

		[HttpPost("login")]
		public async Task<ActionResult<TokenResponseDto>> Login(PostUserDto request)
		{
			TokenResponseDto? result = await authService.Login(request);

			if (result is null)
			{
				return BadRequestProblem("Invalid user or password.");
			}

			return Ok(result);
		}

		[HttpPost("refresh-token")]
		public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
		{
			var result = await authService.RefreshTokensAsync(request);
			if (result is null || result.AccessToken is null || result.RefreshToken is null)
			{
				return UnauthorizedProblem("Invalid refresh token.");
			}
			return Ok(result);
		}
	}
}
