using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace WebGames.Models
{
    [Table("LeaderBoard")]
    public class LeaderBoard
    {
        [Key] public int Id { get; set; }

        public string Username { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
    }

    public class DbLeaderBoard : DbContext
    {
        public DbLeaderBoard() : base("WebGamesContext")
        {
        }

        public DbSet<LeaderBoard> LeaderBoard { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaderBoard>().ToTable("LeaderBoard");
            base.OnModelCreating(modelBuilder);
        }
    }
}