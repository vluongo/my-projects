using ChoirAdminApp.Data;
using ChoirAdminApp.Dtos;
using ChoirAdminApp.Dtos.Chorist;
using ChoirAdminApp.Models;
using ChoirAdminApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChoirAdminApp.Services
{
	public class ChoristService(AppDbContext context, ILogger<DirectorService> logger) : IChoristService
	{
		public async Task<GetChoristDto> AddChorist(PostChoristDto request)
		{
			var newChorist = new Chorist {
				Name = request.Name,
				Birthdate = request.Birthdate,
				Chord = request.Chord,
				FavouriteFood = request.FavouriteFood
			};

			if (request.Choirs != null && request.Choirs.Any())
			{
				ICollection<Choir> choirs = await context.Choirs.Where(c => request.Choirs.Contains(c.ChoirId)).ToListAsync();
				newChorist.Choirs = choirs;
			}

			context.Chorists.Add(newChorist);
			await context.SaveChangesAsync();

			logger.LogInformation("Chorist added {@LogEntry}",
			new
			{
				Operation = "AddChorist",
				newChorist.ChoristID,
				Outcome = "Success",
				Timestamp = DateTime.UtcNow
			});

			return CreateGetChoristDtoFromChorist(newChorist);
		}

		public Task<bool> DeleteChorist(Guid id)
		{
			throw new NotImplementedException();
		}

		public async Task<GetChoristDto?> GetChorist(Guid id)
		{
			Chorist? chorist = await context.Chorists.Include(c => c.Choirs).FirstOrDefaultAsync(c => c.ChoristID == id);
			return chorist is null ? null : CreateGetChoristDtoFromChorist(chorist);
		}

		public Task<PagedResult<GetChoristDto>> GetChorists(QueryParameters parameters)
		{
			throw new NotImplementedException();
		}

		public Task<bool> UpdateChorist(Guid id, PutChoristDto request)
		{
			throw new NotImplementedException();
		}

		private static GetChoristDto CreateGetChoristDtoFromChorist(Chorist chorist)
		{
			var result = new GetChoristDto
			{
				Id = chorist.ChoristID,
				Name = chorist.Name,
				Chord = chorist.Chord,
				Birthdate = chorist.Birthdate,
				FavouriteFood = chorist.FavouriteFood
			};

			List<GetRelatedChoirDto> choirDtos = chorist.Choirs
				.Select(c => new GetRelatedChoirDto
				{
					Name = c.Name,
					Description = c.Description
				})
				.ToList();

			result.Choirs = choirDtos;
			return result;
		}
	}
}
