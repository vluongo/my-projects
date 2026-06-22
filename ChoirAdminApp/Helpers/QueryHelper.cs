using ChoirAdminApp.Dtos;
using Microsoft.EntityFrameworkCore;

namespace ChoirAdminApp.Helpers
{
	public static class QueryHelper
	{
		public static async Task<PagedResult<TResult>> ApplyQueryAsync<T, TResult>(
			IQueryable<T> query,
			QueryParameters parameters,
			Func<T, TResult> selector)
		{
			// Filtering
			if (parameters.Filters != null)
			{
				foreach (var filter in parameters.Filters)
				{
					var propName = filter.Key;
					var value = filter.Value;

					var propInfo = typeof(T).GetProperty(propName);
					if (propInfo == null) continue;

					if (propInfo.PropertyType == typeof(string))
					{
						query = query.Where(e =>
							EF.Property<string>(e, propName).Contains(value));
					}
					else if (propInfo.PropertyType == typeof(int))
					{
						if (int.TryParse(value, out var intVal))
						{
							query = query.Where(e =>
								EF.Property<int>(e, propName) == intVal);
						}
					}
					else if (propInfo.PropertyType == typeof(bool))
					{
						if (bool.TryParse(value, out var boolVal))
						{
							query = query.Where(e =>
								EF.Property<bool>(e, propName) == boolVal);
						}
					}
					else if (propInfo.PropertyType == typeof(DateTime))
					{
						if (DateTime.TryParse(value, out var dtVal))
						{
							query = query.Where(e =>
								EF.Property<DateTime>(e, propName) > dtVal);
						}
					}
				}
			}

			// Sorting
			if (!string.IsNullOrEmpty(parameters.SortBy))
			{
				query = parameters.SortOrder.ToLower() == "desc"
					? query.OrderByDescending(e => EF.Property<object>(e, parameters.SortBy))
					: query.OrderBy(e => EF.Property<object>(e, parameters.SortBy));
			}

			var totalCount = await query.CountAsync();

			// Materialize first (async DB call)
			var entities = await query
				.Skip((parameters.PageNumber - 1) * parameters.PageSize)
				.Take(parameters.PageSize)
				.ToListAsync();

			// Then project in memory
			var items = entities.Select(selector).ToList();

			return new PagedResult<TResult>
			{
				Items = items,
				TotalCount = totalCount,
				PageNumber = parameters.PageNumber,
				PageSize = parameters.PageSize
			};
		}
	}

}
