using ChoirAdminApp.Dtos;
using ChoirAdminApp.Dtos.Chorist;

namespace ChoirAdminApp.Services.Interfaces
{
	public interface IChoristService
	{
		public Task<PagedResult<GetChoristDto>> GetChorists(QueryParameters parameters);
		public Task<GetChoristDto?> GetChorist(Guid id);
		public Task<GetChoristDto> AddChorist(PostChoristDto request);
		public Task<bool> UpdateChorist(Guid id, PutChoristDto request);
		public Task<bool> DeleteChorist(Guid id);
	}
}
