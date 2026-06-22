using ChoirAdminApp.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace ChoirAdminApp.Middleware
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				LogException(ex);
				await HandleExceptionAsync(context, ex);
			}
		}

		private void LogException(Exception exception)
		{
			// Log outer exception
			_logger.LogError(exception, "Exception caught in middleware: {Message}", exception.Message);

			// Log inner exception if present
			if (exception.InnerException != null)
			{
				_logger.LogError(exception.InnerException,
					"Inner exception details: {Message}",
					exception.InnerException.Message);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			ProblemDetails problem = new ProblemDetails
			{
				Type = "https://httpstatuses.com/500",
				Title = "An unexpected error occurred.",
				Status = (int)HttpStatusCode.InternalServerError,
				Detail = exception.Message,
				Instance = context.Request.Path
			};

			switch (exception)
			{
				case ChoirAlreadyHasDirectorException _:
					problem.Type = "https://httpstatuses.com/409";
					problem.Title = "Conflict";
					problem.Status = (int)HttpStatusCode.Conflict;
					break;

				case DbUpdateConcurrencyException _:
					problem.Type = "https://httpstatuses.com/409";
					problem.Title = "Concurrency Conflict";
					problem.Status = (int)HttpStatusCode.Conflict;
					break;

				case DbUpdateException _:
					problem.Type = "https://httpstatuses.com/500";
					problem.Title = "Database Update Failed";
					problem.Status = (int)HttpStatusCode.InternalServerError;
					break;

				case InvalidOperationException _:
					problem.Type = "https://httpstatuses.com/400";
					problem.Title = "Invalid Operation";
					problem.Status = (int)HttpStatusCode.BadRequest;
					break;

				default:
					// General fallback for any other exception
					problem.Type = "https://httpstatuses.com/500";
					problem.Title = "Server Error";
					problem.Status = (int)HttpStatusCode.InternalServerError;
					break;
			}

			context.Response.ContentType = "application/problem+json";
			context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;

			return context.Response.WriteAsJsonAsync(problem);
		}
	}
}
