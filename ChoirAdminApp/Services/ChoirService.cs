using ChoirAdminApp.Data;
using ChoirAdminApp.Dtos;
using ChoirAdminApp.Dtos.Choir;
using ChoirAdminApp.Exceptions;
using ChoirAdminApp.Helpers;
using ChoirAdminApp.Models;
using ChoirAdminApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ChoirAdminApp.Services
{
	public class ChoirService(AppDbContext context, ILogger<DirectorService> logger) : IChoirService
	{
		public async Task<GetChoirDto> AddChoir(PostChoirDto request)
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
					throw new DirectorNotFoundException("Selected director was not found.");
				}
				newChoir.Director = director;
			}

			if (request.Chorists != null && request.Chorists.Any())
			{
				ICollection<Chorist> chorists = await context.Chorists.Where(c => request.Chorists.Contains(c.ChoristID)).ToListAsync();
				newChoir.Chorists = chorists;
			}

			context.Choirs.Add(newChoir);
			await context.SaveChangesAsync();

			logger.LogInformation("Choir added {@LogEntry}",
			new
			{
				Operation = "AddChoir",
				newChoir.ChoirId,
				Outcome = "Success",
				Timestamp = DateTime.UtcNow
			});

			return CreateGetChoirDtoFromChoir(newChoir);
		}

		public async Task<bool> DeleteChoir(Guid id)
		{
			Choir? choirToDelete = await context.Choirs.FindAsync(id);
			if (choirToDelete is null)
			{
				logger.LogWarning("Delete failed {@LogEntry}",
				new
				{
					Operation = "DeleteChoir",
					ChoirId = id,
					Outcome = "NotFound",
					Timestamp = DateTime.UtcNow
				});

				return false;
			}

			context.Choirs.Remove(choirToDelete);
			await context.SaveChangesAsync();

			logger.LogInformation("Choir deleted {@LogEntry}",
			new
			{
				Operation = "DeleteChoir",
				ChoirId = id,
				Outcome = "Success",
				Timestamp = DateTime.UtcNow
			});

			return true;
		}

		public async Task<GetChoirDto?> GetChoir(Guid id)
		{
			Choir? choir = await context.Choirs.Include(c => c.Chorists).FirstOrDefaultAsync(c => c.ChoirId == id);
			return choir is null ? null : CreateGetChoirDtoFromChoir(choir);
		}

		public async Task<PagedResult<GetChoirDto>> GetChoirs(QueryParameters parameters)
		{
			IQueryable<Choir> query = context.Choirs.Include(c => c.Chorists).AsNoTracking();

			return await QueryHelper.ApplyQueryAsync(query, parameters, d => CreateGetChoirDtoFromChoir(d));
		}

		public async Task<bool> UpdateChoir(Guid id,PutChoirDto request)
		{
			Choir? choirToUpdate = await context.Choirs.Include(c => c.Chorists).FirstOrDefaultAsync(c => c.ChoirId == id);

			if (choirToUpdate is null)
			{
				logger.LogWarning("Update failed {@LogEntry}",
				new
				{
					Operation = "UpdateChoir",
					DirectorId = id,
					Outcome = "NotFound",
					Timestamp = DateTime.UtcNow
				});

				return false;
			}

			if (request.Name != null)
			{
				choirToUpdate.Name = request.Name;
			}

			if (request.Description != null)
			{
				choirToUpdate.Description = request.Description;
			}

			if (request.Director != null)
			{
				choirToUpdate.DirectorID = request.Director;
			}

			if (request.Chorists != null)
			{
				if (request.Chorists.Any())
				{
					List<Guid> existingChorists = choirToUpdate.Chorists.Select(c => c.ChoristID).ToList();

					// Remove deleted
					foreach (var chorist in choirToUpdate.Chorists.ToList())
					{
						if (!request.Chorists.Contains(chorist.ChoristID))
						{
							choirToUpdate.Chorists.Remove(chorist);
						}
					}

					// Add new
					var newChoristIds = request.Chorists.Except(existingChorists);

					foreach (var choristId in newChoristIds)
					{
						var newChorist = await context.Chorists.FindAsync(choristId);
						if (newChorist is null)
						{
							throw new InvalidOperationException("At least one choir was not found.");
						}

						choirToUpdate.Chorists.Add(newChorist);
					}

				}
				else
				{
					choirToUpdate.Chorists.Clear();
				}
			}

			await context.SaveChangesAsync();

			logger.LogInformation("Choir updated {@LogEntry}",
			new
			{
				Operation = "UpdateChoir",
				DirectorId = id,
				Outcome = "Success",
				Timestamp = DateTime.UtcNow
			});

			return true;
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
