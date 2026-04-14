namespace cs449sprint2.Models
{
    public class RecordedAction
    {
        public RecordedActionType ActionType { get; set; }

        public int Size { get; set; }
        public BoardType BoardType { get; set; }

        public string BoardState { get; set; } = string.Empty;

        public int FromRow { get; set; }
        public int FromCol { get; set; }
        public int ToRow { get; set; }
        public int ToCol { get; set; }
    }
}
