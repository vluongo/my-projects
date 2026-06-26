using ChoirAdminApp.Constants;
using ChoirAdminApp.Data;
using ChoirAdminApp.Dtos;
using ChoirAdminApp.Exceptions;
using ChoirAdminApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChoirAdminApp.Services
{
	public class AuthService(AppDbContext context, IConfiguration configuration, ILogger<DirectorService> logger) : IAuthService
	{
		public async Task<TokenResponseDto?> Login(PostUserDto request)
		{
			logger.LogInformation("Login attempt for user {Username}", request.Username);

			User? user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
			if (user is null)
			{
				logger.LogWarning("Login failed: user {Username} not found", request.Username);
				return null;
			}

			if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
				== PasswordVerificationResult.Failed)
			{
				logger.LogWarning("Login failed: invalid password for user {Username}", request.Username);
				return null;
			}

			string accessToken = await CreateToken(user);
			var response = new TokenResponseDto { AccessToken = accessToken, RefreshToken = await RotateRefreshToken(user) };

			logger.LogInformation("Login successful for user {UserId}", user.Id);

			return response;
		}


		private string GenerateRefreshToken()
		{
			var randomNumer = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumer);
			return Convert.ToBase64String(randomNumer);
		}


		private async Task<string> RotateRefreshToken(User user)
		{
			using var transaction = await context.Database.BeginTransactionAsync();
			try
			{
				// Step 1: Generate new token
				string newToken = GenerateRefreshToken();
				var refreshToken = new RefreshToken
				{
					Token = newToken,
					ExpiresAt = DateTime.UtcNow.AddDays(7),
					CreatedAt = DateTime.UtcNow,
					UserID = user.Id
				};
				context.RefreshTokens.Add(refreshToken);
				await context.SaveChangesAsync();

				// Step 2: Invalidate old tokens
				var oldTokens = context.RefreshTokens
					.Where(rt => rt.UserID == user.Id && rt.Token != newToken && rt.RevokedAt == null);
				foreach (var old in oldTokens)
				{
					old.RevokedAt = DateTime.UtcNow;
				}
				await context.SaveChangesAsync();

				await transaction.CommitAsync();
				return newToken;
			}
			catch
			{
				await transaction.RollbackAsync();
				throw;
			}
		}

		public async Task<GetUserDto?> RegisterAsync(PostUserDto request)
		{
			if (await context.Users.AnyAsync(u => u.Username == request.Username))
			{
				return null;
			}

			using var transaction = await context.Database.BeginTransactionAsync();

			try
			{
				// 1. Create the user
				var user = new User();
				var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);
				user.Username = request.Username;
				user.PasswordHash = hashedPassword;

				context.Users.Add(user);
				await context.SaveChangesAsync();

				// 2. Lookup default role (e.g. "Member")
				var defaultRole = await context.Roles
					.FirstOrDefaultAsync(r => r.RoleName == RoleNames.Read);

				if (defaultRole == null)
				{
					throw new DefaultRoleNotFoundException("Default role 'Read' not found.");
				}

				// 3. Assign role to user
				var userRole = new UserRole
				{
					UserID = user.Id,
					RoleID = defaultRole.RoleId
				};

				context.UserRoles.Add(userRole);
				await context.SaveChangesAsync();

				// 4. Commit transaction
				await transaction.CommitAsync();

				logger.LogInformation("User created in {@LogEntry}",
				new
				{
					Operation = "AddUser",
					user.Id,
					Outcome = "Success",
					Timestamp = DateTime.UtcNow
				});

				return new GetUserDto { Id = user.Id, Username = user.Username};

			}
			catch (DbUpdateException)
			{
				await transaction.RollbackAsync();
				throw;
			}

		}

		private async Task<string> CreateToken(User user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name,user.Username),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			};

			var roles = await context.UserRoles.Where(ur => ur.UserID == user.Id).Select(ur => ur.Role.RoleName).ToListAsync();

			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

			var tokenDescriptor = new JwtSecurityToken(
				issuer: configuration.GetValue<string>("AppSettings:Issuer"),
				audience: configuration.GetValue<string>("AppSettings:Audience"),
				claims,
				expires: DateTime.UtcNow.AddDays(1),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
		}

		public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
		{
			logger.LogInformation("Refresh token attempt for user {UserId}", request.UserId);

			var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
			if (user is null)
			{
				logger.LogWarning("Invalid refresh token for user {UserId}", request.UserId);
				return null;
			}

			string accessToken = await CreateToken(user);
			var newToken = new TokenResponseDto { AccessToken = accessToken, RefreshToken = await RotateRefreshToken(user) };

			logger.LogInformation("Refresh token successful for user {UserId}", request.UserId);

			return newToken;
		}

		private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
		{
			var token = await context.RefreshTokens
				.FirstOrDefaultAsync(rt => rt.UserID == userId && rt.Token == refreshToken && rt.ExpiresAt > DateTime.UtcNow && rt.RevokedAt == null);
			if (token is null)
			{
				return null;
			}

			User? user = await context.Users.FindAsync(userId);
			return user;
		}
	}
}
