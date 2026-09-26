using AV.Framework.Core.Board;
using AV.Framework.GameData;
using VContainer.Unity;

namespace AV.Framework.Application
{
    public sealed class BoardInitializer : IStartable
    {
        private readonly BoardFactory boardFactory;
        private readonly BoardData boardData;
        private readonly BoardPresenter boardPresenter;
        private readonly MoveHistory moveHistory;
        private readonly GameSession gameSession;
        private readonly GameFlow gameFlow;

        public Board CurrentBoard { get; private set; }

        public BoardInitializer(
            BoardFactory boardFactory,
            BoardData boardData,
            BoardPresenter boardPresenter,
            MoveHistory moveHistory,
            GameSession gameSession,
            GameFlow gameFlow)
        {
            this.boardFactory = boardFactory;
            this.boardData = boardData;
            this.boardPresenter = boardPresenter;
            this.moveHistory = moveHistory;
            this.gameSession = gameSession;
            this.gameFlow = gameFlow;
        }

        public void Start()
        {
            CurrentBoard = boardFactory.Create(boardData);
            moveHistory.Clear();
            gameSession.Start(boardData.MoveLimit);
            gameFlow.StartGame();
            boardPresenter.Present(CurrentBoard);
        }
    }
}