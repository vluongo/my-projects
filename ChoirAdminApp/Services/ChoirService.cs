using ChoirAdminApp.Data;
using ChoirAdminApp.Dtos;
using ChoirAdminApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChoirAdminApp.Services
{
	public class ChoirService(AppDbContext context) : IChoirService
	{
		public async Task<GetChoirDto> AddChoir(SetChoirDto request)
		{
			var newChoir = new Choir
			{
				Name = request.Name,
				Description = request.Description,
				DirectorID = request.Director
			};

			if (newChoir.DirectorID != null)
			{
				Director? director = await context.Directors.FindAsync(newChoir.DirectorID);
				if (director is null)
				{
					throw new InvalidOperationException("Selected director was not found.");
				}
				newChoir.Director = director;
			}

			if (request.Chorists != null && request.Chorists.Any())
			{
				ICollection<Chorist> chorists = await context.Chorists.Where(c => request.Chorists.Contains(c.ChoristID)).AsNoTracking().ToListAsync();
				newChoir.Chorists = chorists;
			}

			context.Choirs.Add(newChoir);
			await context.SaveChangesAsync();

			return CreateGetChoirDtoFromChoir(newChoir);
		}

		public Task<bool> DeleteChoir(Guid id)
		{
			throw new NotImplementedException();
		}

		public async Task<GetChoirDto?> GetChoir(Guid id)
		{
			Choir? choir = await context.Choirs.FindAsync(id);
			return choir is null ? null : CreateGetChoirDtoFromChoir(choir);
		}

		public Task<List<GetChoirDto>> GetChoirs()
		{
			throw new NotImplementedException();
		}

		public Task<bool> UpdateChoir(SetChoirDto request)
		{
			throw new NotImplementedException();
		}

		private static GetChoirDto CreateGetChoirDtoFromChoir(Choir choir)
		{
			var result = new GetChoirDto
			{
				Id = choir.ChoirId,
				Name = choir.Name,
				Description = choir.Description
			};

			if (choir.Director != null && choir.DirectorID != null)
			{
				result.Director = new GetChoirDirectorDto
				{
					Id = (Guid)choir.DirectorID,
					Name = choir.Director.Name,
					Birthdate = choir.Director.Birthdate
				};
			}

			List<GetChoirChoristDto> choristDtos = choir.Chorists
				.Select(c => new GetChoirChoristDto
				{
					Name = c.Name,
					Birthdate = c.Birthdate,
					Chord = c.Chord,
					FavouriteFood = c.FavouriteFood
				})
				.ToList();

			result.Chorists = choristDtos;
			return result;
		}
	}
}
