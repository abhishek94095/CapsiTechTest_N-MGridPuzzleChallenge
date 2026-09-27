using System;
using System.Collections.Generic;
using AV.Framework.GameData;

namespace AV.Framework.Application
{
    public sealed class LevelController
    {
        private readonly List<BoardData> levels;
        private readonly BoardInitializer boardInitializer;
        private readonly MoveHistory moveHistory;
        private readonly GameSession gameSession;
        private readonly GameFlow gameFlow;
        private readonly MovingPieceController movingPieceController;
        private readonly PowerUpController powerUpController;

        public int CurrentLevelIndex { get; private set; } = -1;
        public int LevelCount => levels.Count;
        public bool HasCurrentLevel => CurrentLevelIndex >= 0 && CurrentLevelIndex < levels.Count;
        public bool HasNextLevel => CurrentLevelIndex + 1 < levels.Count;
        public event Action<int> LevelChanged;

        public LevelController(
            List<BoardData> levels,
            BoardInitializer boardInitializer,
            MoveHistory moveHistory,
            GameSession gameSession,
            GameFlow gameFlow,
            MovingPieceController movingPieceController,
            PowerUpController powerUpController)
        {
            if (levels == null) throw new ArgumentNullException(nameof(levels));
            if (levels.Count == 0) throw new InvalidOperationException("No levels are configured for the game.");

            for (int index = 0; index < levels.Count; index++)
            {
                if (levels[index] == null)
                {
                    throw new InvalidOperationException($"Level at index {index} is null.");
                }
            }

            this.levels = levels;
            this.boardInitializer = boardInitializer ?? throw new ArgumentNullException(nameof(boardInitializer));
            this.moveHistory = moveHistory ?? throw new ArgumentNullException(nameof(moveHistory));
            this.gameSession = gameSession ?? throw new ArgumentNullException(nameof(gameSession));
            this.gameFlow = gameFlow ?? throw new ArgumentNullException(nameof(gameFlow));
            this.movingPieceController = movingPieceController ?? throw new ArgumentNullException(nameof(movingPieceController));
            this.powerUpController = powerUpController ?? throw new ArgumentNullException(nameof(powerUpController));
        }

        public bool StartLevel(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= levels.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(levelIndex), levelIndex, $"Level index {levelIndex} is out of range.");
            }

            BoardData boardData = levels[levelIndex];
            if (boardData == null) throw new InvalidOperationException($"Level at index {levelIndex} is null.");

            movingPieceController.StopLevel();
            powerUpController.ResetForLevel();
            moveHistory.Clear();

            boardInitializer.Initialize(boardData);
            gameSession.Start(GetMoveLimit(levelIndex));

            CurrentLevelIndex = levelIndex;
            LevelChanged?.Invoke(CurrentLevelIndex + 1);

            gameFlow.StartGame();
            movingPieceController.StartLevel();
            return true;
        }

        public bool RetryLevel()
        {
            if (!HasCurrentLevel) return false;
            return StartLevel(CurrentLevelIndex);
        }

        public bool NextLevel()
        {
            if (gameFlow.State != GameFlowState.Won) return false;
            if (!HasNextLevel) return false;

            return StartLevel(CurrentLevelIndex + 1);
        }

        public void ReturnToMainMenu()
        {
            movingPieceController.StopLevel();
            moveHistory.Clear();
            gameFlow.ReturnToMenu();
        }

        private int GetMoveLimit(int levelIndex)
        {
            int moveLimit = levels[levelIndex].MoveLimit;
            if (moveLimit < 1)
            {
                throw new InvalidOperationException($"Level at index {levelIndex} has an invalid move limit.");
            }

            return moveLimit;
        }
    }
}
