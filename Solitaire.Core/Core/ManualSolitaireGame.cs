namespace cs449sprint2.Core
{
    public class ManualSolitaireGame : SolitaireGameBase
    {
        private readonly Random _random = new();

        public void RandomizeBoard()
        {
            for (int r = 0; r < Board.Size; r++)
            {
                for (int c = 0; c < Board.Size; c++)
                {
                    if (Board.IsPlayablePosition(r, c))
                    {
                        Board.SetCell(r, c, _random.Next(2) == 0 ? Models.CellState.Peg : Models.CellState.Empty);
                    }
                }
            }

            if (IsRecording)
                Recorder.RecordBoardState(Board.SerializeState());
        }
    }
}
