using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace WebGames.Models
{
    /// <summary>
    /// Represents a leaderboard entry in the WebGames application.
    /// </summary>
    [Table("LeaderBoard")]
    public class LeaderBoard
    {
        /// <summary>
        /// Gets or sets the ID of the leaderboard entry.
        /// </summary>
        [Key] public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the player.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the number of wins of the player.
        /// </summary>
        public int Wins { get; set; }

        /// <summary>
        /// Gets or sets the number of losses of the player.
        /// </summary>
        public int Losses { get; set; }

        /// <summary>
        /// Gets or sets the number of ties of the player.
        /// </summary>
        public int Ties { get; set; }
    }

    /// <summary>
    /// Represents the database context for the leaderboard in the WebGames application.
    /// </summary>
    public class DbLeaderBoard : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the DbLeaderBoard class.
        /// </summary>
        public DbLeaderBoard() : base("WebGamesContext")
        {
        }

        /// <summary>
        /// Gets or sets the leaderboard entries.
        /// </summary>
        public DbSet<LeaderBoard> LeaderBoard { get; set; }

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types
        /// exposed in DbSet properties on your derived context. 
        /// </summary>
        /// <param name="modelBuilder">Provides a simple API surface for configuring a DbContext.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaderBoard>().ToTable("LeaderBoard");
            base.OnModelCreating(modelBuilder);
        }
    }
}