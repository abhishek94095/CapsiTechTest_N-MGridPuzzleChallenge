using System;
using AV.Framework.Core.Board;
using AV.Framework.GameData;

namespace AV.Framework.Application
{
    public sealed class BoardInitializer : IDisposable
    {
        private readonly BoardFactory boardFactory;
        private readonly BoardPresenter boardPresenter;
        private readonly GameFlow gameFlow;

        public Board CurrentBoard { get; private set; }

        public BoardInitializer(
            BoardFactory boardFactory,
            BoardPresenter boardPresenter,
            GameFlow gameFlow)
        {
            this.boardFactory = boardFactory ?? throw new ArgumentNullException(nameof(boardFactory));
            this.boardPresenter = boardPresenter ?? throw new ArgumentNullException(nameof(boardPresenter));
            this.gameFlow = gameFlow ?? throw new ArgumentNullException(nameof(gameFlow));
            this.gameFlow.StateChanged += OnGameFlowStateChanged;
        }

        public void Dispose()
        {
            gameFlow.StateChanged -= OnGameFlowStateChanged;
        }

        public void Initialize(BoardData boardData)
        {
            if (boardData == null) throw new ArgumentNullException(nameof(boardData));

            CurrentBoard = boardFactory.Create(boardData);
            boardPresenter.Present(CurrentBoard);
        }

        private void OnGameFlowStateChanged(GameFlowState state)
        {
            if (state == GameFlowState.Playing) return;

            boardPresenter.Clear();
        }
    }
}