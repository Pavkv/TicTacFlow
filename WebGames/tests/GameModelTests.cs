using WebGames.Models;
using Xunit;

namespace WebGames.tests
{
    public class GameModelTests
    {
        [Fact]
        public void SetPlayerChoice_WithPlayerX_SetsHumanPlayerToXAndAiPlayerToO()
        {
            var game = new Game();
            game.SetPlayerChoice(Player.X);

            Assert.Equal(Player.X, game.HumanPlayer);
            Assert.Equal(Player.O, game.AiPlayer);
        }

        [Fact]
        public void SetPlayerChoice_WithPlayerO_SetsHumanPlayerToOAndAiPlayerToX()
        {
            var game = new Game();
            game.SetPlayerChoice(Player.O);

            Assert.Equal(Player.O, game.HumanPlayer);
            Assert.Equal(Player.X, game.AiPlayer);
        }

        [Fact]
        public void MakeMove_WithValidMove_ReturnsTrueAndUpdatesBoard()
        {
            var game = new Game();
            var result = game.MakeMove(0, 0, Player.X);

            Assert.True(result);
            Assert.Equal(Player.X, game.Board[0, 0]);
        }

        [Fact]
        public void MakeMove_WithInvalidMove_ReturnsFalseAndDoesNotUpdateBoard()
        {
            var game = new Game
            {
                Board =
                {
                    [0, 0] = Player.X
                }
            };
            var result = game.MakeMove(0, 0, Player.O);

            Assert.False(result);
            Assert.Equal(Player.X, game.Board[0, 0]);
        }

        [Fact]
        public void CheckWin_WithWinningBoard_ReturnsTrue()
        {
            var game = new Game
            {
                Board =
                {
                    [0, 0] = Player.X,
                    [0, 1] = Player.X,
                    [0, 2] = Player.X
                },
                CurrentPlayer = Player.X
            };

            Assert.True(game.CheckWin());
        }

        [Fact]
        public void CheckWin_WithNonWinningBoard_ReturnsFalse()
        {
            var game = new Game
            {
                Board =
                {
                    [0, 0] = Player.X,
                    [0, 1] = Player.X,
                    [0, 2] = Player.O
                },
                CurrentPlayer = Player.X
            };

            Assert.False(game.CheckWin());
        }

        [Fact]
        public void IsBoardFull_WithFullBoard_ReturnsTrue()
        {
            var game = new Game();
            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
                game.Board[i, j] = Player.X;

            Assert.True(game.IsBoardFull());
        }

        [Fact]
        public void IsBoardFull_WithNonFullBoard_ReturnsFalse()
        {
            var game = new Game();
            for (var i = 0; i < 2; i++)
            for (var j = 0; j < 3; j++)
                game.Board[i, j] = Player.X;

            Assert.False(game.IsBoardFull());
        }
    }
}