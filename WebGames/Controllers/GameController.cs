using System;
using System.Linq;
using System.Web.Mvc;
using WebGames.Models;

namespace WebGames.Controllers
{
    public class GameController : Controller
    {
        /// <summary>
        /// Gets the game from the session.
        /// </summary>
        private Game _game => GetGameFromSession();

        /// <summary>
        /// Action for displaying the game view.
        /// </summary>
        /// <returns>The game view.</returns>
        public ActionResult Game()
        {
            var game = new Game();
            return View(game);
        }
        
        /// <summary>
        /// Action for choosing a side in the game.
        /// </summary>
        /// <param name="side">The chosen side.</param>
        /// <returns>A JSON result indicating the success of the action and the AI's move if applicable.</returns>
        [HttpPost]
        public ActionResult ChooseSide(string side)
        {
            var playerChoice = side.ToUpper() == "X" ? Player.X : Player.O;
            _game.SetPlayerChoice(playerChoice);
            if (_game.HumanPlayer != Player.O || _game.Difficulty == Difficulty.None)
                return Json(new { success = true });
            var aiMove = _game.PerformAiMove();
            _game.CurrentPlayer = _game.HumanPlayer;
            return Json(new
            {
                success = true, AIMoved = true, User = _game.HumanPlayer, AImoveRow = aiMove[0], AImoveCol = aiMove[1]
            });
        }

        /// <summary>
        /// Action for choosing a difficulty in the game.
        /// </summary>
        /// <param name="difficulty">The chosen difficulty.</param>
        /// <returns>A JSON result indicating the success of the action and the AI's move if applicable.</returns>
        [HttpPost]
        public ActionResult ChooseDifficulty(string difficulty)
        {
            if (_game.HumanPlayer == Player.None)
                return Json(new { success = false, message = "Please choose a side first" });

            _game.Difficulty = (Difficulty)Enum.Parse(typeof(Difficulty), difficulty);

            if (_game.HumanPlayer != Player.O) return Json(new { success = true });

            var aiMove = _game.PerformAiMove();
            _game.CurrentPlayer = _game.HumanPlayer;

            return Json(new
            {
                success = true, AIMoved = true, User = _game.HumanPlayer, AImoveRow = aiMove[0], AImoveCol = aiMove[1]
            });
        }

        /// <summary>
        /// Action for making a move in the game.
        /// </summary>
        /// <param name="row">The row of the move.</param>
        /// <param name="col">The column of the move.</param>
        /// <returns>A JSON result indicating the success of the action and the AI's move if applicable.</returns>
        [HttpPost]
        public JsonResult Game(int row, int col)
        {
            Console.WriteLine(row);
            if (_game.HumanPlayer == Player.None)
                return Json(new { success = false, message = "Please choose a side first" });
            if (_game.Difficulty == Difficulty.None)
                return Json(new { success = false, message = "Please choose a difficulty first" });
            var user = _game.CurrentPlayer == Player.X ? "X" : "O";
            var username = GetUsername();

            if (!_game.MakeMove(row, col, _game.HumanPlayer))
                return Json(new { success = false, message = "Invalid move" });

            if (CheckGameOver(username))
                return Json(new
                    { success = true, message = GetWinMessage(_game), User = user, AIMoved = false, gameOver = true });

            _game.SwitchPlayer();
            var aiMove = _game.PerformAiMove();

            if (CheckGameOver(username))
                return Json(new
                {
                    success = true, message = GetWinMessage(_game), AIMoved = true, User = user, AImoveRow = aiMove[0],
                    AImoveCol = aiMove[1], gameOver = true
                });

            _game.SwitchPlayer();
            return Json(new
            {
                success = true, AIMoved = true, User = user, AImoveRow = aiMove[0], AImoveCol = aiMove[1],
                gameOver = false
            });
        }

        /// <summary>
        /// Checks if the game is over.
        /// </summary>
        /// <param name="username">The username of the player.</param>
        /// <returns>True if the game is over, false otherwise.</returns>
        private bool CheckGameOver(string username)
        {
            if (!_game.GameOver && !_game.IsBoardFull()) return false;

            if (!User.Identity.IsAuthenticated) return true;
            if (username == null) return true;
            if (_game.CheckWin() && _game.CurrentPlayer == _game.HumanPlayer)
                AddLeaderBoardEntry(username, 1, 0, 0);
            else if (_game.CheckWin() && _game.CurrentPlayer != _game.HumanPlayer)
                AddLeaderBoardEntry(username, 0, 1, 0);
            else
                AddLeaderBoardEntry(username, 0, 0, 1);

            return true;
        }
        
        /// <summary>
        /// Gets the win message for the game.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <returns>The win message.</returns>
        private static string GetWinMessage(Game game)
        {
            return game.CheckWin() ? $"{Enum.GetName(typeof(Player), game.CurrentPlayer)} wins!" : "It's a tie!";
        }
        
        /// <summary>
        /// Adds an entry to the leaderboard.
        /// </summary>
        /// <param name="username">The username of the player.</param>
        /// <param name="wins">The number of wins.</param>
        /// <param name="losses">The number of losses.</param>
        /// <param name="ties">The number of ties.</param>
        private static void AddLeaderBoardEntry(string username, int wins, int losses, int ties)
        {
            using (var db = new DbLeaderBoard())
            {
                var entry = db.LeaderBoard.FirstOrDefault(e => e.Username == username) ??
                            new LeaderBoard { Username = username };
                Console.WriteLine(entry.Username);
                entry.Wins += wins;
                entry.Losses += losses;
                entry.Ties += ties;

                if (entry.Id == 0) db.LeaderBoard.Add(entry);

                db.SaveChanges();
            }
        }
        
        /// <summary>
        /// Gets the username of the player.
        /// </summary>
        /// <returns>The username of the player.</returns>
        private string GetUsername()
        {
            using (var db = new DbUserAccount())
            {
                var user = db.UserAccounts.FirstOrDefault(u => u.Username == User.Identity.Name);
                return user?.Username;
            }
        }
        
        /// <summary>
        /// Action for displaying the leaderboard.
        /// </summary>
        /// <returns>The leaderboard view.</returns>
        public ActionResult LeaderBoard()
        {
            using (var db = new DbLeaderBoard())
            {
                var leaderBoard = db.LeaderBoard.OrderByDescending(e => e.Wins).ToList();
                return View(leaderBoard);
            }
        }

        /// <summary>
        /// Action for resetting the game.
        /// </summary>
        /// <returns>A JSON result indicating the success of the action.</returns>
        [HttpPost]
        public JsonResult ResetGame()
        {
            Session["Game"] = new Game();
            return Json(new { success = true, message = "Game has been reset." });
        }
        
        /// <summary>
        /// Gets the game from the session.
        /// </summary>
        /// <returns>The game from the session.</returns>
        public Game GetGameFromSession()
        {
            if (Session["Game"] == null) Session["Game"] = new Game();
            return (Game)Session["Game"];
        }
    }
}