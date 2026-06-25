using ChoirAdminApp.Constants;
using ChoirAdminApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChoirAdminApp.Data
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
		public DbSet<Chorist> Chorists { get; set; }
		public DbSet<Choir> Choirs { get; set; }
		public DbSet<Director> Directors { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Role>	Roles { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Chorist>().ToTable(nameof(Chorist))
				.HasMany(c => c.Choirs)
				.WithMany(i => i.Chorists);

			modelBuilder.Entity<Choir>()
			  .HasOne(c => c.Director)
			  .WithMany(d => d.Choirs)
			  .HasForeignKey(c => c.DirectorID)
			  .OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserID, ur.RoleID });

			modelBuilder.Entity<Choir>().ToTable(nameof(Choir));
			modelBuilder.Entity<Director>().ToTable(nameof(Director));
			modelBuilder.Entity<User>().ToTable(nameof(User));
			modelBuilder.Entity<Role>().ToTable(nameof(Role));
			modelBuilder.Entity<UserRole>().ToTable(nameof(UserRole));
			modelBuilder.Entity<RefreshToken>().ToTable(nameof(RefreshToken));

			var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
			modelBuilder.Entity<Role>().HasData(
				new Role { RoleId = adminRoleId, RoleName = "Admin" },
				new Role { RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222"), RoleName = RoleNames.Read },
				new Role { RoleId = Guid.Parse("33333333-3333-3333-3333-333333333333"), RoleName = RoleNames.Create },
				new Role { RoleId = Guid.Parse("44444444-4444-4444-4444-444444444444"), RoleName = RoleNames.Edit },
				new Role { RoleId = Guid.Parse("55555555-5555-5555-5555-555555555555"), RoleName = RoleNames.Delete }
			);

			var adminUserId = Guid.Parse("66666666-6666-6666-6666-666666666666"); 

			modelBuilder.Entity<User>().HasData(new User
			{
				Id = adminUserId,
				Username = "admin",
				PasswordHash = "PLACEHOLDER" // will be replaced at runtime
			});

			modelBuilder.Entity<UserRole>().HasData(
				new UserRole { UserID = adminUserId, RoleID = adminRoleId }
			);
		}

	}
}
