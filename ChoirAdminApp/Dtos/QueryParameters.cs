using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ChoirAdminApp.Dtos
{
	public class QueryParameters
	{
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 2;
		public string? SortBy { get; set; }
		public string SortOrder { get; set; } = "asc";
		[FromQuery(Name = "Filters")]
		public string? FiltersJson { get; set; }
		public Dictionary<string, string>? Filters =>
			string.IsNullOrEmpty(FiltersJson)
				? null
				: JsonSerializer.Deserialize<Dictionary<string, string>>(FiltersJson);
	}
}
