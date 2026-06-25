using ChoirAdminApp.Data;
using ChoirAdminApp.Filters;
using ChoirAdminApp.Middleware;
using ChoirAdminApp.Models;
using ChoirAdminApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
	options.Filters.Add<ValidateModelAttribute>();
}).ConfigureApiBehaviorOptions(options =>
{
	// Prevent ASP.NET Core from automatically returning 400 with validation details
	options.SuppressModelStateInvalidFilter = true;
}).AddJsonOptions(options =>
{
	// Automatically converts all C# enums to JSON strings
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["AppSettings:Issuer"],
		ValidateAudience = true,
		ValidAudience = builder.Configuration["AppSettings:Audience"],
		ValidateLifetime = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Token")!)),
		ValidateIssuerSigningKey = true
	};
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
																//.EnableSensitiveDataLogging(false) // keep PII out
																//.LogTo(Log.Information, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information));
builder.Services.AddScoped<IDirectorService, DirectorService>();
builder.Services.AddScoped<IChoirService, ChoirService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Basic Serilog configuration
Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console() // plain console output
	.WriteTo.File("logs/api-.log", rollingInterval: RollingInterval.Day)
	.CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
	app.MapScalarApiReference();
	app.UseDeveloperExceptionPage();
}

using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");

	if (adminUser != null && adminUser.PasswordHash == "PLACEHOLDER")
	{
		var passwordHasher = new PasswordHasher<User>();
		var adminPassword = builder.Configuration["AdminPassword"]; // from secrets or appsettings.json

		adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminPassword);
		await context.SaveChangesAsync();
	}
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
