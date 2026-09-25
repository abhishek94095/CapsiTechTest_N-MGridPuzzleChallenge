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

        public Board CurrentBoard { get; private set; }

        public BoardInitializer(BoardFactory boardFactory, BoardData boardData, BoardPresenter boardPresenter)
        {
            this.boardFactory = boardFactory;
            this.boardData = boardData;
            this.boardPresenter = boardPresenter;
        }

        public void Start()
        {
            CurrentBoard = boardFactory.Create(boardData);
            boardPresenter.Present(CurrentBoard);
        }
    }
}