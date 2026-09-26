using System;
using AV.Framework.Core.Board;
using MessagePipe;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AV.Framework.Application
{
    public sealed class GameHud : MonoBehaviour, IDisposable
    {
        [SerializeField] private TextMeshProUGUI remainingMovesText;
        [SerializeField] private Button undoButton;

        private GameSession gameSession;
        private IPublisher<UndoRequestedEvent> undoPublisher;

        public void Initialize(GameSession gameSession, IPublisher<UndoRequestedEvent> undoPublisher)
        {
            this.gameSession = gameSession ?? throw new ArgumentNullException(nameof(gameSession));
            this.undoPublisher = undoPublisher ?? throw new ArgumentNullException(nameof(undoPublisher));
            if (remainingMovesText == null) throw new InvalidOperationException("Remaining Moves Text is not assigned.");
            if (undoButton == null) throw new InvalidOperationException("Undo Button is not assigned.");

            gameSession.RemainingMovesChanged += OnRemainingMovesChanged;
            undoButton.onClick.AddListener(OnUndoClicked);
            OnRemainingMovesChanged(gameSession.RemainingMoves);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (gameSession != null)
            {
                gameSession.RemainingMovesChanged -= OnRemainingMovesChanged;
            }

            undoButton?.onClick.RemoveListener(OnUndoClicked);
        }

        private void OnRemainingMovesChanged(int remainingMoves)
        {
            remainingMovesText.text = $"Moves: {remainingMoves}";
        }

        private void OnUndoClicked()
        {
            undoPublisher.Publish(new UndoRequestedEvent());
        }
    }
}
