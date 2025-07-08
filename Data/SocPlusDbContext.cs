#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
using SocPlus.Models;

namespace SocPlus.Data;

public class SocPlusDbContext : DbContext {
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Vote> Votes { get; set; }


    public SocPlusDbContext(DbContextOptions<SocPlusDbContext> options) : base(options) {
        
    }

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
    }
}
