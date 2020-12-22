using Microsoft.EntityFrameworkCore;
using ZoomersClient.Shared.Models;

namespace ZoomersClient.Server.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasMany(x => x.AnsweredQuestions);
        }
        
        public DbSet<Game> Games { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<QuestionBase> Questions { get; set; }

        public DbSet<AnsweredQuestion> Answers { get; set; }
    }
}