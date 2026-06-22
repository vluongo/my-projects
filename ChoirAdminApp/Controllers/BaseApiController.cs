using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChoirAdminApp.Controllers
{
	[ApiController]
	public class BaseApiController : ControllerBase
	{
		// For typed results (ActionResult<T>)
		protected ActionResult<T> ProblemResult<T>(
			int statusCode,
			string title,
			string detail = null,
			string? type = null)
		{
			var problem = new ProblemDetails
			{
				Status = statusCode,
				Title = title,
				Detail = detail,
				Type = type ?? $"https://httpstatuses.com/{statusCode}"
			};

			return new ObjectResult(problem)
			{
				StatusCode = statusCode,
				ContentTypes = { "application/problem+json" }
			};
		}

		// For untyped results (ActionResult)
		protected ActionResult ProblemResult(
			int statusCode,
			string title,
			string detail = null,
			string? type = null)
		{
			var problem = new ProblemDetails
			{
				Status = statusCode,
				Title = title,
				Detail = detail,
				Type = type ?? $"https://httpstatuses.com/{statusCode}"
			};

			return new ObjectResult(problem)
			{
				StatusCode = statusCode,
				ContentTypes = { "application/problem+json" }
			};
		}

		// Convenience wrappers
		protected ActionResult<T> NotFoundProblem<T>(string detail = null)
			=> ProblemResult<T>(StatusCodes.Status404NotFound, "Entity not found", detail);

		protected ActionResult NotFoundProblem(string detail = null)
			=> ProblemResult(StatusCodes.Status404NotFound, "Entity not found", detail);

		protected ActionResult<T> ConflictProblem<T>(string detail = null)
			=> ProblemResult<T>(StatusCodes.Status409Conflict, "Conflict occurred", detail);

		protected ActionResult ConflictProblem(string detail = null)
			=> ProblemResult(StatusCodes.Status409Conflict, "Conflict occurred", detail);
	}
}
