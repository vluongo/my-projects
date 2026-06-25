using ChoirAdminApp.Dtos;
using ChoirAdminApp.Models;

namespace ChoirAdminApp.Services
{
	public interface IAuthService
	{
		Task<GetUserDto?> RegisterAsync(PostUserDto request);
		Task<TokenResponseDto?> Login(PostUserDto request);
		Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
		//	TODO: add roles management
	}
}
