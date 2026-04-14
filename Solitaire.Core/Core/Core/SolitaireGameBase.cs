using cs449sprint2.Models;

namespace cs449sprint2.Core
{
    public abstract class SolitaireGameBase
    {
        public Board Board { get; protected set; } = null!;

        protected GameRecordingService Recorder { get; } = new();

        public bool IsRecording => Recorder.IsRecording;

        public virtual void StartNewGame(int size, BoardType type)
        {
            Board = new Board(size, type);
        }

        public void StartRecording(string filePath)
        {
            if (Board == null)
                throw new InvalidOperationException("Start a game before recording.");

            Recorder.Start(filePath);
            Recorder.RecordNewGame(Board.Size, Board.Type, Board.SerializeState());
        }

        public void StopRecording()
        {
            Recorder.StopAndSave();
        }

        public void ReplayFromFile(string filePath)
        {
            List<RecordedAction> actions = GameRecordingService.LoadFromFile(filePath);

            foreach (RecordedAction action in actions)
            {
                switch (action.ActionType)
                {
                    case RecordedActionType.NewGame:
                        StartNewGame(action.Size, action.BoardType);
                        break;

                    case RecordedActionType.BoardState:
                        Board.LoadState(action.BoardState);
                        break;

                    case RecordedActionType.Move:
                        MakeMove(action.FromRow, action.FromCol, action.ToRow, action.ToCol);
                        break;
                }
            }
        }

        public virtual bool IsValidMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            if (Board == null)
                return false;

            if (!Board.IsInsideBounds(fromRow, fromCol) || !Board.IsInsideBounds(toRow, toCol))
                return false;

            if (Board.GetCell(fromRow, fromCol) != CellState.Peg)
                return false;

            if (Board.GetCell(toRow, toCol) != CellState.Empty)
                return false;

            int dr = toRow - fromRow;
            int dc = toCol - fromCol;

            bool validDistance =
                (Math.Abs(dr) == 2 && dc == 0) ||
                (Math.Abs(dc) == 2 && dr == 0);

            if (!validDistance)
                return false;

            int middleRow = (fromRow + toRow) / 2;
            int middleCol = (fromCol + toCol) / 2;

            if (!Board.IsInsideBounds(middleRow, middleCol))
                return false;

            return Board.GetCell(middleRow, middleCol) == CellState.Peg;
        }

        public virtual bool MakeMove(int fromRow, int fromCol, int toRow, int toCol)
        {
            if (!IsValidMove(fromRow, fromCol, toRow, toCol))
                return false;

            int middleRow = (fromRow + toRow) / 2;
            int middleCol = (fromCol + toCol) / 2;

            Board.SetCell(fromRow, fromCol, CellState.Empty);
            Board.SetCell(middleRow, middleCol, CellState.Empty);
            Board.SetCell(toRow, toCol, CellState.Peg);

            if (IsRecording)
            {
                Recorder.RecordMove(fromRow, fromCol, toRow, toCol);
            }

            return true;
        }

        public virtual List<Move> GetValidMoves()
        {
            var moves = new List<Move>();

            if (Board == null)
                return moves;

            for (int r = 0; r < Board.Size; r++)
            {
                for (int c = 0; c < Board.Size; c++)
                {
                    if (Board.GetCell(r, c) != CellState.Peg)
                        continue;

                    TryAddMove(moves, r, c, r + 2, c);
                    TryAddMove(moves, r, c, r - 2, c);
                    TryAddMove(moves, r, c, r, c + 2);
                    TryAddMove(moves, r, c, r, c - 2);
                }
            }

            return moves;
        }

        private void TryAddMove(List<Move> moves, int fromRow, int fromCol, int toRow, int toCol)
        {
            if (IsValidMove(fromRow, fromCol, toRow, toCol))
            {
                moves.Add(new Move
                {
                    FromRow = fromRow,
                    FromCol = fromCol,
                    ToRow = toRow,
                    ToCol = toCol
                });
            }
        }

        public virtual bool IsGameOver()
        {
            return GetValidMoves().Count == 0;
        }
    }
}
