using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace WebGames.Models
{
    [Table("UserAccounts")]
    public class UserAccount
    {
        [Key] public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class DbUserAccount : DbContext
    {
        public DbUserAccount() : base("WebGamesContext")
        {
        }

        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccount>().ToTable("UserAccounts");
            base.OnModelCreating(modelBuilder);
        }
    }
}