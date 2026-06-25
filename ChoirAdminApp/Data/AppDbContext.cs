using ChoirAdminApp.Models;
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
		}

	}
}
