using System;
using System.Collections.Generic;
using System.Linq;

namespace WebGames.Models
{
    public class Game
    {
        private readonly Dictionary<string, int> _minimaxCache = new Dictionary<string, int>();
        public readonly Player[,] Board = new Player[3, 3];

        public Game()
        {
            for (var i = 0; i < 9; i++)
                Board[i / 3, i % 3] = Player.None;
        }

        public Player HumanPlayer { get; private set; } = Player.None;
        public Player AiPlayer { get; set; } = Player.None;
        public Player CurrentPlayer { get; set; } = Player.None;
        public bool GameOver { get; private set; }
        public Difficulty Difficulty { get; set; } = Difficulty.None;

        public void SetPlayerChoice(Player playerChoice)
        {
            HumanPlayer = playerChoice;
            AiPlayer = playerChoice == Player.X ? Player.O : Player.X;
            CurrentPlayer = Player.X;
        }

        public bool MakeMove(int row, int col, Player player)
        {
            if (row < 0 || row >= 3 || col < 0 || col >= 3 || Board[row, col] != Player.None || GameOver) return false;
            Board[row, col] = player;
            if (CheckWin())
                GameOver = true;
            else if (IsBoardFull()) GameOver = true;

            return true;
        }

        public void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        }

        public bool CheckWin()
        {
            for (var i = 0; i < 3; i++)
                if (CheckLine(i, 0, 0, 1) || CheckLine(0, i, 1, 0))
                    return true;
            return CheckLine(0, 0, 1, 1) || CheckLine(0, 2, 1, -1);
        }

        private bool CheckLine(int startRow, int startCol, int rowStep, int colStep)
        {
            return Enumerable.Range(0, 3)
                .All(i => Board[startRow + i * rowStep, startCol + i * colStep] == CurrentPlayer);
        }

        public bool IsBoardFull()
        {
            return Board.Cast<Player>().All(cell => cell != Player.None);
        }

        public int[] PerformAiMove()
        {
            switch (Difficulty)
            {
                case Difficulty.Easy:
                    return AiMoveEasy();
                case Difficulty.Medium:
                    return AiMoveMedium();
                case Difficulty.Hard:
                    return AiMoveHard();
                case Difficulty.None:
                default:
                    return AiMoveMedium();
            }
        }

        private int[] AiMoveEasy()
        {
            var emptyCells = new List<int[]>();
            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
                if (Board[i, j] == Player.None)
                    emptyCells.Add(new[] { i, j });

            var random = new Random();
            var index = random.Next(emptyCells.Count);
            Board[emptyCells[index][0], emptyCells[index][1]] = AiPlayer;
            return emptyCells[index];
        }

        private int[] AiMoveMedium()
        {
            var moves = new List<Func<int[]>>
                { AiWinningMove, BlockHumanWinningMove, TakeCenter, TakeCorner, TakeSide };
            foreach (var result in moves.Select(move => move.Invoke()).Where(result => result != null)) return result;

            return new[] { 0, 0 };
        }

        private int[] AiWinningMove()
        {
            return WinningMove(AiPlayer);
        }

        private int[] BlockHumanWinningMove()
        {
            return WinningMove(HumanPlayer);
        }

        private int[] WinningMove(Player player)
        {
            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
            {
                if (Board[i, j] != Player.None) continue;
                Board[i, j] = player;
                if (CheckWin())
                {
                    Board[i, j] = AiPlayer;
                    return new[] { i, j };
                }

                Board[i, j] = Player.None;
            }

            return null;
        }

        private int[] TakeCenter()
        {
            if (Board[1, 1] != Player.None) return null;
            Board[1, 1] = AiPlayer;
            return new[] { 1, 1 };
        }

        private int[] TakeCorner()
        {
            return TakePosition(new[,] { { 0, 0 }, { 0, 2 }, { 2, 0 }, { 2, 2 } });
        }

        private int[] TakeSide()
        {
            return TakePosition(new[,] { { 0, 1 }, { 1, 0 }, { 1, 2 }, { 2, 1 } });
        }

        private int[] TakePosition(int[,] positions)
        {
            for (var i = 0; i < positions.GetLength(0); i++)
            {
                var row = positions[i, 0];
                var col = positions[i, 1];
                if (Board[row, col] != Player.None) continue;
                Board[row, col] = AiPlayer;
                return new[] { row, col };
            }

            return null;
        }

        private int[] AiMoveHard()
        {
            var bestMove = new[] { -1, -1 };
            var bestScore = int.MinValue;
            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
            {
                if (Board[i, j] != Player.None) continue;
                Board[i, j] = AiPlayer;
                var score = Minimax(Board, 0, false);
                Board[i, j] = Player.None;
                if (score <= bestScore) continue;
                bestScore = score;
                bestMove = new[] { i, j };
            }

            if (bestMove[0] != -1) MakeMove(bestMove[0], bestMove[1], AiPlayer);

            return bestMove;
        }

        private int Minimax(Player[,] board, int depth, bool isMaximizing)
        {
            var boardKey = GetBoardKey(board);
            if (_minimaxCache.TryGetValue(boardKey, out var cachedScore)) return cachedScore;

            if (CheckWin())
            {
                var score = isMaximizing ? -10 + depth : 10 - depth;
                _minimaxCache[boardKey] = score;
                return score;
            }

            if (IsBoardFull())
            {
                _minimaxCache[boardKey] = 0;
                return 0;
            }

            var bestScore = isMaximizing ? int.MinValue : int.MaxValue;
            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
            {
                if (board[i, j] != Player.None) continue;
                board[i, j] = isMaximizing ? AiPlayer : HumanPlayer;
                var score = Minimax(board, depth + 1, !isMaximizing);
                board[i, j] = Player.None;
                bestScore = isMaximizing ? Math.Max(score, bestScore) : Math.Min(score, bestScore);
            }

            _minimaxCache[boardKey] = bestScore;
            return bestScore;
        }


        private static string GetBoardKey(Player[,] board)
        {
            var key = "";
            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
                key += board[i, j];

            return key;
        }

        public override string ToString()
        {
            var result = "";
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++) result += Board[i, j] + " ";

                result += "\n";
            }

            return result;
        }
    }
}