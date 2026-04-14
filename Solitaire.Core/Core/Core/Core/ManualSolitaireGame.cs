using cs449sprint2.Models;

namespace cs449sprint2.Core
{
    public class ManualSolitaireGame : SolitaireGameBase
    {
        private readonly Random _random = new();

        public void RandomizeBoard()
        {
            if (Board == null)
                throw new InvalidOperationException("Start a game before randomizing the board.");

            int maxAttempts = 50;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                RandomizePlayableCells();

                if (Board.CountPegs() > 1 && GetValidMoves().Count > 0)
                {
                    if (IsRecording)
                    {
                        Recorder.RecordBoardState(Board.SerializeState());
                    }

                    return;
                }
            }

            if (IsRecording)
            {
                Recorder.RecordBoardState(Board.SerializeState());
            }
        }

        private void RandomizePlayableCells()
        {
            var playablePositions = new List<(int Row, int Col)>();

            for (int r = 0; r < Board.Size; r++)
            {
                for (int c = 0; c < Board.Size; c++)
                {
                    if (Board.IsPlayablePosition(r, c))
                        playablePositions.Add((r, c));
                }
            }

            if (playablePositions.Count == 0)
                return;

            foreach (var pos in playablePositions)
            {
                Board.SetCell(pos.Row, pos.Col, _random.Next(2) == 0 ? CellState.Peg : CellState.Empty);
            }

            if (Board.CountPegs() == playablePositions.Count)
            {
                var emptySpot = playablePositions[_random.Next(playablePositions.Count)];
                Board.SetCell(emptySpot.Row, emptySpot.Col, CellState.Empty);
            }

            if (Board.CountPegs() < 2)
            {
                int needed = 2 - Board.CountPegs();

                while (needed > 0)
                {
                    var pegSpot = playablePositions[_random.Next(playablePositions.Count)];

                    if (Board.GetCell(pegSpot.Row, pegSpot.Col) != CellState.Peg)
                    {
                        Board.SetCell(pegSpot.Row, pegSpot.Col, CellState.Peg);
                        needed--;
                    }
                }
            }
        }
    }
}
