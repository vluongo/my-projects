using ChoirAdminApp.Dtos;
using ChoirAdminApp.Models;

namespace ChoirAdminApp.Services.Interfaces
{
	public interface IAuthService
	{
		Task<GetUserDto?> RegisterAsync(PostUserDto request);
		Task<TokenResponseDto?> Login(PostUserDto request);
		Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
	}
}
