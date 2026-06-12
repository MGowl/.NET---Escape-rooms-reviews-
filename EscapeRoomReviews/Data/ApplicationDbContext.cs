using EscapeRoomReviews.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EscapeRoomReviews.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> AppUsers => Set<User>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<EscapeRoom> EscapeRooms => Set<EscapeRoom>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Theme> Themes => Set<Theme>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EscapeRoom>()
            .HasOne(er => er.Location)
            .WithMany(l => l.EscapeRooms)
            .HasForeignKey(er => er.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<EscapeRoom>()
            .HasOne(er => er.Company)
            .WithMany(c => c.EscapeRooms)
            .HasForeignKey(er => er.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Photo>()
            .HasOne(p => p.EscapeRoom)
            .WithMany(er => er.Photos)
            .HasForeignKey(p => p.EscapeRoomId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.EscapeRoom)
            .WithMany(er => er.Reviews)
            .HasForeignKey(r => r.EscapeRoomId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<EscapeRoom>()
            .HasMany(er => er.Themes)
            .WithMany(t => t.EscapeRooms);
    }
}
