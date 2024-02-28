using System.ComponentModel.DataAnnotations;

namespace WebGames.Models
{
    /// <summary>
    /// Represents a login model in the WebGames application.
    /// </summary>
    public class Login
    {
        /// <summary>
        /// Gets or sets the username or email of the user.
        /// </summary>
        [Required]
        [Display(Name = "Username or Email")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user should be remembered.
        /// </summary>
        [Display(Name = "Remember me")] 
        public bool RememberMe { get; set; }
    }
}