using System.Linq;
using System.Web.Mvc;
using Xunit;
using Moq;
using WebGames.Controllers;
using WebGames.Models;

namespace WebGames.tests
{
    public class AccountControllerTests
    {
        private readonly Mock<DbUserAccount> _mockDb = new Mock<DbUserAccount>();
        private readonly AccountController _controller = new AccountController();

        [Fact]
        public void SignIn_WithValidModelAndNonExistingUser_RedirectsToLogin()
        {
            var model = new SignUp { Username = "test", Password = "password" };

            var result = _controller.SignUp(model) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Login", result.RouteValues["action"]);
            Assert.Equal("Account", result.RouteValues["controller"]);
        }

        [Fact]
        public void SignIn_WithInvalidModel_ReturnsViewWithModel()
        {
            var model = new SignUp { Username = "", Password = "password" };

            var result = _controller.SignUp(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public void SignIn_WithExistingUser_ReturnsViewWithModel()
        {
            var model = new SignUp { Username = "existingUser", Password = "password" };
            _mockDb.Setup(db => db.UserAccounts.Any(u => u.Username == model.Username)).Returns(true);

            var result = _controller.SignUp(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public void Login_WithValidModelAndExistingUser_RedirectsToGame()
        {
            var model = new Login { Username = "existingUser", Password = "password" };
            _mockDb.Setup(db => db.UserAccounts.FirstOrDefault(u => u.Username == model.Username)).Returns(
                new UserAccount
                    { Username = model.Username, Password = BCrypt.Net.BCrypt.HashPassword(model.Password) });

            var result = _controller.Login(model, null) as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Game", result.RouteValues["action"]);
            Assert.Equal("Game", result.RouteValues["controller"]);
        }

        [Fact]
        public void Login_WithInvalidModel_ReturnsViewWithModel()
        {
            var model = new Login { Username = "", Password = "password" };

            var result = _controller.Login(model, null) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public void Login_WithNonExistingUser_ReturnsViewWithModel()
        {
            var model = new Login { Username = "nonExistingUser", Password = "password" };

            var result = _controller.Login(model, null) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public void LogOut_Always_RedirectsToGame()
        {
            var result = _controller.LogOut() as RedirectToRouteResult;

            Assert.NotNull(result);
            Assert.Equal("Game", result.RouteValues["action"]);
            Assert.Equal("Game", result.RouteValues["controller"]);
        }
    }
}