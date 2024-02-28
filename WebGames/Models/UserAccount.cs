using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace WebGames.Models
{
    /// <summary>
    /// Represents a user account in the WebGames application.
    /// </summary>
    [Table("UserAccounts")]
    public class UserAccount
    {
        /// <summary>
        /// Gets or sets the ID of the user account.
        /// </summary>
        [Key] public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user account.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the user account.
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Represents the database context for the user accounts in the WebGames application.
    /// </summary>
    public class DbUserAccount : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the DbUserAccount class.
        /// </summary>
        public DbUserAccount() : base("WebGamesContext")
        {
        }

        /// <summary>
        /// Gets or sets the user accounts.
        /// </summary>
        public DbSet<UserAccount> UserAccounts { get; set; }

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types
        /// exposed in DbSet properties on your derived context.
        /// </summary>
        /// <param name="modelBuilder">Provides a simple API surface for configuring a DbContext.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccount>().ToTable("UserAccounts");
            base.OnModelCreating(modelBuilder);
        }
    }
}