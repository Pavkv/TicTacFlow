using System;
using System.Collections.Generic;
using System.Linq;

namespace WebGames.Models
{
    /// <summary>
    /// Represents a game in the application.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Cache for the minimax algorithm.
        /// </summary>
        private readonly Dictionary<string, int> _minimaxCache = new Dictionary<string, int>();

        /// <summary>
        /// Represents the game board.
        /// </summary>
        public readonly Player[,] Board = new Player[3, 3];

        /// <summary>
        /// Initializes a new instance of the Game class.
        /// </summary>
        public Game()
        {
            for (var i = 0; i < 9; i++)
                Board[i / 3, i % 3] = Player.None;
        }

        /// <summary>
        /// Gets or sets the human player.
        /// </summary>
        public Player HumanPlayer { get; private set; } = Player.None;

        /// <summary>
        /// Gets or sets the AI player.
        /// </summary>
        public Player AiPlayer { get; set; } = Player.None;

        /// <summary>
        /// Gets or sets the current player.
        /// </summary>
        public Player CurrentPlayer { get; set; } = Player.None;

        /// <summary>
        /// Gets a value indicating whether the game is over.
        /// </summary>
        public bool GameOver { get; private set; }

        /// <summary>
        /// Gets or sets the difficulty of the game.
        /// </summary>
        public Difficulty Difficulty { get; set; } = Difficulty.None;

        /// <summary>
        /// Sets the player's choice.
        /// </summary>
        /// <param name="playerChoice">The player's choice.</param>
        public void SetPlayerChoice(Player playerChoice)
        {
            HumanPlayer = playerChoice;
            AiPlayer = playerChoice == Player.X ? Player.O : Player.X;
            CurrentPlayer = Player.X;
        }

        /// <summary>
        /// Makes a move in the game.
        /// </summary>
        /// <param name="row">The row of the move.</param>
        /// <param name="col">The column of the move.</param>
        /// <param name="player">The player making the move.</param>
        /// <returns>True if the move was successful, false otherwise.</returns>
        public bool MakeMove(int row, int col, Player player)
        {
            if (row < 0 || row >= 3 || col < 0 || col >= 3 || Board[row, col] != Player.None || GameOver) return false;
            Board[row, col] = player;
            if (CheckWin())
                GameOver = true;
            else if (IsBoardFull()) GameOver = true;

            return true;
        }

        /// <summary>
        /// Switches the current player.
        /// </summary>
        public void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        }

        /// <summary>
        /// Checks if there is a win condition on the board.
        /// </summary>
        /// <returns>True if there is a win condition, false otherwise.</returns>
        public bool CheckWin()
        {
            for (var i = 0; i < 3; i++)
                if (CheckLine(i, 0, 0, 1) || CheckLine(0, i, 1, 0))
                    return true;
            return CheckLine(0, 0, 1, 1) || CheckLine(0, 2, 1, -1);
        }

        /// <summary>
        /// Checks if there is a line of the same player on the board.
        /// </summary>
        /// <param name="startRow">The starting row of the line.</param>
        /// <param name="startCol">The starting column of the line.</param>
        /// <param name="rowStep">The step in the row direction.</param>
        /// <param name="colStep">The step in the column direction.</param>
        /// <returns>True if there is a line, false otherwise.</returns>
        private bool CheckLine(int startRow, int startCol, int rowStep, int colStep)
        {
            return Enumerable.Range(0, 3)
                .All(i => Board[startRow + i * rowStep, startCol + i * colStep] == CurrentPlayer);
        }

        /// <summary>
        /// Checks if the board is full.
        /// </summary>
        /// <returns>True if the board is full, false otherwise.</returns>
        public bool IsBoardFull()
        {
            return Board.Cast<Player>().All(cell => cell != Player.None);
        }

        /// <summary>
        /// Performs the AI's move.
        /// </summary>
        /// <returns>The row and column of the move.</returns>
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

        /// <summary>
        /// Performs an easy AI move by randomly selecting an empty cell on the board.
        /// </summary>
        /// <returns>The row and column of the selected cell.</returns>
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

        /// <summary>
        /// Performs a medium difficulty AI move by following a set of strategies in order.
        /// </summary>
        /// <returns>The row and column of the selected cell.</returns>
        private int[] AiMoveMedium()
        {
            var moves = new List<Func<int[]>>
                { AiWinningMove, BlockHumanWinningMove, TakeCenter, TakeCorner, TakeSide };
            foreach (var result in moves.Select(move => move.Invoke()).Where(result => result != null)) return result;

            return new[] { 0, 0 };
        }

        /// <summary>
        /// Finds a winning move for the AI player.
        /// </summary>
        /// <returns>The row and column of the winning move, or null if no winning move is found.</returns>
        private int[] AiWinningMove()
        {
            return WinningMove(AiPlayer);
        }

        /// <summary>
        /// Finds a move that blocks the human player from winning.
        /// </summary>
        /// <returns>The row and column of the blocking move, or null if no blocking move is found.</returns>
        private int[] BlockHumanWinningMove()
        {
            return WinningMove(HumanPlayer);
        }

        /// <summary>
        /// Finds a winning move for a given player.
        /// </summary>
        /// <param name="player">The player to find a winning move for.</param>
        /// <returns>The row and column of the winning move, or null if no winning move is found.</returns>
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

        /// <summary>
        /// Takes the center cell if it is empty.
        /// </summary>
        /// <returns>The row and column of the center cell, or null if the center cell is not empty.</returns>
        private int[] TakeCenter()
        {
            if (Board[1, 1] != Player.None) return null;
            Board[1, 1] = AiPlayer;
            return new[] { 1, 1 };
        }

        /// <summary>
        /// Takes a corner cell if one is empty.
        /// </summary>
        /// <returns>The row and column of the selected corner cell, or null if no corner cell is empty.</returns>
        private int[] TakeCorner()
        {
            return TakePosition(new[,] { { 0, 0 }, { 0, 2 }, { 2, 0 }, { 2, 2 } });
        }

        /// <summary>
        /// Takes a side cell if one is empty.
        /// </summary>
        /// <returns>The row and column of the selected side cell, or null if no side cell is empty.</returns>
        private int[] TakeSide()
        {
            return TakePosition(new[,] { { 0, 1 }, { 1, 0 }, { 1, 2 }, { 2, 1 } });
        }

        /// <summary>
        /// Takes a position from a given set of positions.
        /// </summary>
        /// <param name="positions">The set of positions to choose from.</param>
        /// <returns>The row and column of the selected position, or null if none of the positions are empty.</returns>
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

        /// <summary>
        /// Performs a hard difficulty AI move by using the minimax algorithm.
        /// </summary>
        /// <returns>The row and column of the selected cell.</returns>
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

        /// <summary>
        /// Performs the minimax algorithm to find the best move for the AI player.
        /// </summary>
        /// <param name="board">The current state of the game board.</param>
        /// <param name="depth">The current depth of the game tree.</param>
        /// <param name="isMaximizing">True if the current move is for the AI player, false if it is for the human player.</param>
        /// <returns>The score of the best move.</returns>
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

        /// <summary>
        /// Generates a key for the current state of the game board.
        /// </summary>
        /// <param name="board">The current state of the game board.</param>
        /// <returns>A string representing the current state of the game board.</returns>
        private static string GetBoardKey(Player[,] board)
        {
            var key = "";
            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
                key += board[i, j];

            return key;
        }

        /// <summary>
        /// Returns a string representation of the game board.
        /// </summary>
        /// <returns>A string representing the current state of the game board.</returns>
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