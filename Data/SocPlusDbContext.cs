using Microsoft.EntityFrameworkCore;
using SocPlus.Models;

namespace SocPlus.Data;

public class SocPlusDbContext(DbContextOptions<SocPlusDbContext> options) : DbContext(options) {
    
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Follow> Follows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Vote>().HasKey(v => new {v.UserId, v.PostId});
        modelBuilder
            .Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder
            .Entity<Vote>()
            .HasOne(v => v.User)
            .WithMany(u => u.Votes)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany()
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Follow>().HasKey(f => new { f.UserId, f.FollowerId });
    }
}
