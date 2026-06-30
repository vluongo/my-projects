using ChoirAdminApp.Data;
using ChoirAdminApp.Dtos;
using ChoirAdminApp.Dtos.Chorist;
using ChoirAdminApp.Helpers;
using ChoirAdminApp.Models;
using ChoirAdminApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

		public async Task<bool> DeleteChorist(Guid id)
		{
			Chorist? choristToDelete = await context.Chorists.FindAsync(id);
			if (choristToDelete == null)
			{
				logger.LogWarning("Delete failed {@LogEntry}",
				new
				{
					Operation = "DeleteChorist",
					ChoristId = id,
					Outcome = "NotFound",
					Timestamp = DateTime.UtcNow
				});

				return false;
			}

			context.Chorists.Remove(choristToDelete);
			await context.SaveChangesAsync();

			logger.LogInformation("Chorist deleted {@LogEntry}",
			new
			{
				Operation = "DeleteChorist",
				ChoristId = id,
				Outcome = "Success",
				Timestamp = DateTime.UtcNow
			});

			return true;
		}

		public async Task<GetChoristDto?> GetChorist(Guid id)
		{
			Chorist? chorist = await context.Chorists.Include(c => c.Choirs).FirstOrDefaultAsync(c => c.ChoristID == id);
			return chorist is null ? null : CreateGetChoristDtoFromChorist(chorist);
		}

		public async Task<PagedResult<GetChoristDto>> GetChorists(QueryParameters parameters)
		{
			IQueryable<Chorist> query = context.Chorists.Include(c => c.Choirs).AsNoTracking();

			return await QueryHelper.ApplyQueryAsync(query, parameters, d => CreateGetChoristDtoFromChorist(d));
		}

		public async Task<bool> UpdateChorist(Guid id, PutChoristDto request)
		{
			Chorist? choristToUpdate = await context.Chorists.Include(c => c.Choirs).FirstOrDefaultAsync(c => c.ChoristID == id);
			if (choristToUpdate is null) {
				logger.LogWarning("Update failed {@LogEntry}",
				new
				{
					Operation = "UpdateChorist",
					ChoristId = id,
					Outcome = "NotFound",
					Timestamp = DateTime.UtcNow
				});

				return false;
			}

			if (request.Name != null)
			{
				choristToUpdate.Name = request.Name;
			}

			if (request.Birthdate != null)
			{
				choristToUpdate.Birthdate = (DateTime)request.Birthdate;
			}

			if (request.FavouriteFood != null)
			{
				choristToUpdate.FavouriteFood = request.FavouriteFood;
			}

			if (request.Chord != null)
			{
				choristToUpdate.Chord = choristToUpdate.Chord;
			}

			if (request.Choirs != null)
			{
				if (request.Choirs.Any())
				{
					List<Guid> existingChoirs = choristToUpdate.Choirs.Select(c => c.ChoirId).ToList();

					// Remove deleted
					foreach (var choir in choristToUpdate.Choirs.ToList())
					{
						if (!request.Choirs.Contains(choir.ChoirId))
						{
							choristToUpdate.Choirs.Remove(choir);
						}
					}

					// Add new
					var newChoirIds = request.Choirs.Except(existingChoirs);

					foreach (var choirId in newChoirIds)
					{
						var newChoir = await context.Choirs.FindAsync(choirId);
						if (newChoir is null)
						{
							throw new InvalidOperationException("At least one choir was not found.");
						}

						choristToUpdate.Choirs.Add(newChoir);
					}

				}
				else
				{
					choristToUpdate.Choirs.Clear();
				}
			}

			await context.SaveChangesAsync();

			logger.LogInformation("Chorist updated {@LogEntry}",
			new
			{
				Operation = "UpdateChorist",
				ChoristId = id,
				Outcome = "Success",
				Timestamp = DateTime.UtcNow
			});

			return true;
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
