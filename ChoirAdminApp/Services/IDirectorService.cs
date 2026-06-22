using ChoirAdminApp.Dtos;
using ChoirAdminApp.Models;

namespace ChoirAdminApp.Services
{
	public interface IDirectorService
	{
		public Task<PagedResult<GetDirectorDto>> GetDirectors(QueryParameters parameters);
		public Task<GetDirectorDto?> GetDirector(Guid id);
		public Task<GetDirectorDto> AddDirector(PostDirectorDto request);
		public Task<bool> UpdateDirector(Guid id,PutDirectorDto request);
		public Task<bool> DeleteDirector(Guid id);
	}
}
