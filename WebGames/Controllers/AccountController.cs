using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebGames.Models;

namespace WebGames.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbUserAccount _db = new DbUserAccount();

        public ActionResult SignIn(SignIn model)
        {
            if (!ModelState.IsValid || UserExists(model.Username))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(model);
            }

            CreateUser(model);
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Login(Login model, string returnUrl)
        {
            Console.WriteLine(UserExists("Lorens"));
            if (!ModelState.IsValid || !ValidateUser(model))
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            Console.WriteLine(model.Username);
            FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
            Console.WriteLine(FormsAuthentication.FormsCookieName);
            return RedirectToGameOrUrl(returnUrl);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Game", "Game");
        }

        private bool UserExists(string username)
        {
            return _db.UserAccounts.Any(u => u.Username == username);
        }

        private void CreateUser(SignIn model)
        {
            _db.UserAccounts.Add(new UserAccount
            {
                Username = model.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            });
            _db.SaveChanges();
        }

        private bool ValidateUser(Login model)
        {
            var user = _db.UserAccounts.FirstOrDefault(u => u.Username == model.Username);
            return user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password);
        }

        private ActionResult RedirectToGameOrUrl(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Game", "Game");
        }
    }
}