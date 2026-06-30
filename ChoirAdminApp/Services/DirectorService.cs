using ChoirAdminApp.Data;
using ChoirAdminApp.Dtos;
using ChoirAdminApp.Dtos.Director;
using ChoirAdminApp.Exceptions;
using ChoirAdminApp.Helpers;
using ChoirAdminApp.Models;
using ChoirAdminApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace ChoirAdminApp.Services
{
	public class DirectorService(AppDbContext context, ILogger<DirectorService> logger) : IDirectorService
	{

		public async Task<GetDirectorDto> AddDirector(PostDirectorDto request)
		{
			var newDirector = new Director
			{
				Name = request.Name,
				Birthdate = request.Birthdate
			};

			if (request.Choirs != null && request.Choirs.Any())
			{

				ICollection<Choir> choirs = await context.Choirs.Where(c => request.Choirs.Contains(c.ChoirId)).ToListAsync();

				if (choirs.Any(c => c.Director != null || c.DirectorID != null))
				{
					throw new ChoirAlreadyHasDirectorException("One or more choirs already have a director.");
				}

				foreach (var choir in choirs)
				{
					choir.Director = newDirector;
					choir.DirectorID = newDirector.DirectorId;
				}

			}

			context.Directors.Add(newDirector);
			await context.SaveChangesAsync();

			logger.LogInformation("Director added {@LogEntry}",
			new
			{
				Operation = "AddDirector",
				newDirector.DirectorId,
				Outcome = "Success",
				Timestamp = DateTime.UtcNow
			});

			return CreateGetDirectorDtoFromDirector(newDirector);
		}


		public async Task<bool> DeleteDirector(Guid id)
		{
			Director? director = await context.Directors.FindAsync(id);
			if (director is null)
			{
				logger.LogWarning("Delete failed {@LogEntry}",
				new
				{
					Operation = "DeleteDirector",
					DirectorId = id,
					Outcome = "NotFound",
					Timestamp = DateTime.UtcNow
				});
				return false;
			}

			context.Directors.Remove(director);
			await context.SaveChangesAsync();

			logger.LogInformation("Director deleted {@LogEntry}",
			new
			{
				Operation = "DeleteDirector",
				DirectorId = id,
				Outcome = "Success",
				Timestamp = DateTime.UtcNow
			});

			return true;
		}


		public async Task<GetDirectorDto?> GetDirector(Guid id)
		{
			Director? director = await context.Directors.FindAsync(id);
			return director is null ? null : CreateGetDirectorDtoFromDirector(director);
		}


		public async Task<PagedResult<GetDirectorDto>> GetDirectors(QueryParameters parameters)
		{
			IQueryable<Director> query = context.Directors.Include(d => d.Choirs).AsNoTracking();

			return await QueryHelper.ApplyQueryAsync(query, parameters, d => CreateGetDirectorDtoFromDirector(d));
		}

		public async Task<bool> UpdateDirector(Guid id,PutDirectorDto request)
		{
			Director? directorToUpdate = await context.Directors.Include(d => d.Choirs).FirstOrDefaultAsync(d => d.DirectorId == id);
			if (directorToUpdate is null) {
				logger.LogWarning("Update failed {@LogEntry}",
				new
				{
					Operation = "UpdateDirector",
					DirectorId = id,
					Outcome = "NotFound",
					Timestamp = DateTime.UtcNow
				});

				return false;
			}

			if (request.Name != null)
			{
				directorToUpdate.Name = request.Name;
			}

			if (request.Birthdate != null)
			{
				directorToUpdate.Birthdate = (DateTime)request.Birthdate;
			}

			if (request.Choirs != null)
			{
				if (request.Choirs.Any())
				{
					List<Guid> existingChoirs = directorToUpdate.Choirs.Select(c => c.ChoirId).ToList();

					// Remove deleted
					foreach (var choir in directorToUpdate.Choirs.ToList())
					{
						if (!request.Choirs.Contains(choir.ChoirId))
						{
							choir.DirectorID = null; // unlink
							choir.Director = null;
							directorToUpdate.Choirs.Remove(choir);
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

						newChoir.Director = directorToUpdate;
						newChoir.DirectorID = id;
					}
					
				}
				else
				{
					foreach (var choir in directorToUpdate.Choirs.ToList())
					{
						choir.DirectorID = null;
					}
				}
			}

			await context.SaveChangesAsync();
			
			logger.LogInformation("Director updated {@LogEntry}",
			new
			{
				Operation = "UpdateDirector",
				DirectorId = id,
				Outcome = "Success",
				Timestamp = DateTime.UtcNow
			});

			return true;
		}


		private static GetDirectorDto CreateGetDirectorDtoFromDirector(Director director)
		{
			var result = new GetDirectorDto
			{
				Id = director.DirectorId,
				Name = director.Name,
				Birthdate = director.Birthdate
			};

			List<GetDirectorChoirDto> choirDtos = director.Choirs
				.Select(c => new GetDirectorChoirDto
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
