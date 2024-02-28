using System;
using System.Linq;
using System.Web.Mvc;
using WebGames.Models;

namespace WebGames.Controllers
{
    public class GameController : Controller
    {
        private Game _game => GetGameFromSession();

        public ActionResult Game()
        {
            var game = new Game();
            return View(game);
        }

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

        private static string GetWinMessage(Game game)
        {
            return game.CheckWin() ? $"{Enum.GetName(typeof(Player), game.CurrentPlayer)} wins!" : "It's a tie!";
        }

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

        private string GetUsername()
        {
            using (var db = new DbUserAccount())
            {
                var user = db.UserAccounts.FirstOrDefault(u => u.Username == User.Identity.Name);
                return user?.Username;
            }
        }

        public ActionResult LeaderBoard()
        {
            using (var db = new DbLeaderBoard())
            {
                var leaderBoard = db.LeaderBoard.OrderByDescending(e => e.Wins).ToList();
                return View(leaderBoard);
            }
        }

        [HttpPost]
        public JsonResult ResetGame()
        {
            Session["Game"] = new Game();
            return Json(new { success = true, message = "Game has been reset." });
        }

        public Game GetGameFromSession()
        {
            if (Session["Game"] == null) Session["Game"] = new Game();
            return (Game)Session["Game"];
        }
    }
}