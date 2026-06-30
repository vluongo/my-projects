using ChoirAdminApp.Dtos;
using ChoirAdminApp.Dtos.Choir;

namespace ChoirAdminApp.Services.Interfaces
{
	public interface IChoirService
	{
		public Task<PagedResult<GetChoirDto>> GetChoirs(QueryParameters parameters);
		public Task<GetChoirDto?> GetChoir(Guid id);
		public Task<GetChoirDto> AddChoir(PostChoirDto request);
		public Task<bool> UpdateChoir(Guid id, PutChoirDto request);
		public Task<bool> DeleteChoir(Guid id);
	}
}
