using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Models;

namespace VotingSystem.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollOption> PollOptions { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Poll>()
                .HasMany(p => p.Options)
                .WithOne(o => o.Poll)
                .HasForeignKey(o => o.PollId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Poll>()
                .HasMany(p => p.Votes)
                .WithOne(v => v.Poll)
                .HasForeignKey(v => v.PollId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<PollOption>()
                .HasMany(o => o.Votes)
                .WithOne(v => v.PollOption)
                .HasForeignKey(v => v.PollOptionId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<User>()
                .HasMany(u => u.Votes)
                .WithOne(v => v.User)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}