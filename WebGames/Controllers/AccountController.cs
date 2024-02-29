using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebGames.Models;

namespace WebGames.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbUserAccount _db = new DbUserAccount();

        /// <summary>
        /// Action for signing in a user.
        /// </summary>
        /// <param name="model">The sign in model.</param>
        /// <returns>An ActionResult that redirects to the login action if successful, otherwise returns the view with the model.</returns>
        public ActionResult SignUp(SignUp model)
        {
            if (!ModelState.IsValid || UserExists(model.Username))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(model);
            }

            CreateUser(model);
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Action for logging in a user.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <param name="returnUrl">The URL to redirect to after successful login.</param>
        /// <returns>An ActionResult that redirects to the game or the provided URL if successful, otherwise returns the view with the model.</returns>
        public ActionResult Login(Login model, string returnUrl)
        {
            if (!ModelState.IsValid || !ValidateUser(model))
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
            return RedirectToGameOrUrl(returnUrl);
        }

        /// <summary>
        /// Action for logging out a user.
        /// </summary>
        /// <returns>An ActionResult that redirects to the game.</returns>
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Game", "Game");
        }

        /// <summary>
        /// Checks if a user exists.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>True if the user exists, false otherwise.</returns>
        private bool UserExists(string username)
        {
            return _db.UserAccounts.Any(u => u.Username == username);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="model">The sign in model.</param>
        private void CreateUser(SignUp model)
        {
            _db.UserAccounts.Add(new UserAccount
            {
                Username = model.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            });
            _db.SaveChanges();
        }

        /// <summary>
        /// Validates a user's login credentials.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <returns>True if the credentials are valid, false otherwise.</returns>
        private bool ValidateUser(Login model)
        {
            var user = _db.UserAccounts.FirstOrDefault(u => u.Username == model.Username);
            return user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
        }

        /// <summary>
        /// Redirects to the game or the provided URL.
        /// </summary>
        /// <param name="returnUrl">The URL to redirect to.</param>
        /// <returns>An ActionResult that redirects to the game or the provided URL.</returns>
        private ActionResult RedirectToGameOrUrl(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Game", "Game");
        }
    }
}