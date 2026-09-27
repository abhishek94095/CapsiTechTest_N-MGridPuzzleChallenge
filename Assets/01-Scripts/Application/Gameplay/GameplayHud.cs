using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AV.Framework.Core.Board;
using MessagePipe;
using VContainer;

namespace AV.Framework.Application
{
    public sealed class GameplayHud : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI remainingMovesText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button backButton;

        private GameSession gameSession;
        private MoveHistory moveHistory;
        private LevelController levelController;
        private GameFlow gameFlow;
        private IPublisher<UndoRequestedEvent> undoPublisher;

        [Inject]
        private void Initialize(
            GameSession gameSession,
            MoveHistory moveHistory,
            LevelController levelController,
            GameFlow gameFlow,
            IPublisher<UndoRequestedEvent> undoPublisher)
        {
            this.gameSession = gameSession;
            this.moveHistory = moveHistory;
            this.levelController = levelController;
            this.gameFlow = gameFlow;
            this.undoPublisher = undoPublisher;

            gameSession.RemainingMovesChanged += OnRemainingMovesChanged;
            moveHistory.CountChanged += OnMoveHistoryChanged;
            levelController.LevelChanged += OnLevelChanged;
            gameFlow.StateChanged += OnGameFlowStateChanged;

            ConfigureButtons();
            OnRemainingMovesChanged(gameSession.RemainingMoves);
            OnMoveHistoryChanged(moveHistory.Count);
            OnLevelChanged(levelController.CurrentLevelIndex + 1);
            SetVisible(gameFlow.State == GameFlowState.Playing);
        }

        private void OnDestroy()
        {
            if (gameSession != null) gameSession.RemainingMovesChanged -= OnRemainingMovesChanged;
            if (moveHistory != null) moveHistory.CountChanged -= OnMoveHistoryChanged;
            if (levelController != null) levelController.LevelChanged -= OnLevelChanged;
            if (gameFlow != null) gameFlow.StateChanged -= OnGameFlowStateChanged;
            undoButton?.onClick.RemoveListener(OnUndoClicked);
            backButton?.onClick.RemoveListener(OnBackClicked);
        }

        private void ConfigureButtons()
        {
            if (undoButton != null)
            {
                undoButton.onClick.AddListener(OnUndoClicked);
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
        }

        private void OnUndoClicked()
        {
            undoPublisher.Publish(new UndoRequestedEvent());
        }

        private void OnBackClicked()
        {
            levelController.ReturnToMainMenu();
        }

        private void OnRemainingMovesChanged(int remainingMoves)
        {
            if (remainingMovesText != null) remainingMovesText.text = $"Moves: {remainingMoves}";
        }

        private void OnMoveHistoryChanged(int moveCount)
        {
            if (undoButton != null) undoButton.interactable = moveCount > 0 && gameFlow.State == GameFlowState.Playing;
        }

        private void OnLevelChanged(int levelNumber)
        {
            if (levelText != null) levelText.text = $"Level {levelNumber}";
        }

        private void OnGameFlowStateChanged(GameFlowState state)
        {
            SetVisible(state == GameFlowState.Playing);
            OnMoveHistoryChanged(moveHistory.Count);
        }

        private void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }
    }
}
