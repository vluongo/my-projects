using ChoirAdminApp.Dtos;

namespace ChoirAdminApp.Services
{
	public interface IChoirService
	{
		public Task<List<GetChoirDto>> GetChoirs();
		public Task<GetChoirDto?> GetChoir(Guid id);
		public Task<GetChoirDto> AddChoir(SetChoirDto request);
		public Task<bool> UpdateChoir(SetChoirDto request);
		public Task<bool> DeleteChoir(Guid id);
	}
}
