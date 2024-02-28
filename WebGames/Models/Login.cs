using System.ComponentModel.DataAnnotations;

namespace WebGames.Models
{
    public class Login
    {
        [Required]
        [Display(Name = "Username or Email")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")] public bool RememberMe { get; set; }
    }
}