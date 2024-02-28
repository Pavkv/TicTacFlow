using Xunit;
using Moq;
using WebGames.Controllers;
using System.Web.Mvc;

namespace WebGames.tests
{
    public class GameControllerTests
    {
        private Mock<ControllerContext> _mockContext;
        private GameController _controller;

        public GameControllerTests()
        {
            _mockContext = new Mock<ControllerContext>();
            _controller = new GameController { ControllerContext = _mockContext.Object };
        }

        [Fact]
        public void Game_WhenCalled_ReturnsViewResult()
        {
            var result = _controller.Game();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ChooseSide_WithValidSide_ReturnsJsonResult()
        {
            var result = _controller.ChooseSide("X");

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public void ChooseDifficulty_WithValidDifficulty_ReturnsJsonResult()
        {
            var result = _controller.ChooseDifficulty("Easy");

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public void Game_WithValidMove_ReturnsJsonResult()
        {
            var result = _controller.Game(1, 1);

            Assert.IsType<JsonResult>(result);
        }

        [Fact]
        public void LeaderBoard_WhenCalled_ReturnsViewResult()
        {
            var result = _controller.LeaderBoard();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void ResetGame_WhenCalled_ReturnsJsonResult()
        {
            var result = _controller.ResetGame();

            Assert.IsType<JsonResult>(result);
        }
    }
}