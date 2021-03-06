using Microsoft.EntityFrameworkCore;
using ZoomersClient.Shared.Models;

namespace ZoomersClient.Shared.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        
        public DbSet<Game> Games { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<GameQuestion> Questions { get; set; }

        public DbSet<AnsweredQuestion> Answers { get; set; }

        public DbSet<QuestionBase> AllQuestions { get; set; }
    }
}