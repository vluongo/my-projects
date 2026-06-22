using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChoirAdminApp.Filters
{
	public class ValidateModelAttribute : ActionFilterAttribute
	{
		private readonly ILogger<ValidateModelAttribute> _logger;

		public ValidateModelAttribute(ILogger<ValidateModelAttribute> logger)
		{
			_logger = logger;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.ModelState.IsValid)
			{
				var errorMessages = context.ModelState
					.SelectMany(ms => ms.Value.Errors.Select(e => $"{ms.Key}: {e.ErrorMessage}"))
					.ToList();

				_logger.LogWarning("Model binding errors: {Errors}", string.Join("\n", errorMessages));


				context.Result = new BadRequestObjectResult(new { Message = "Invalid request payload." });
			}
		}
	}

}
