using System.Text;
using cs449sprint2.Models;

namespace cs449sprint2.Core
{
    public class Board
    {
        public int Size { get; }
        public BoardType Type { get; }
        private readonly CellState[,] _cells;

        public Board(int size, BoardType type)
        {
            if (size < 3 || size % 2 == 0)
                throw new ArgumentException("Board size must be an odd number greater than or equal to 3.");

            Size = size;
            Type = type;
            _cells = new CellState[size, size];

            Initialize();
        }

        private void Initialize()
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    _cells[r, c] = IsPlayablePosition(r, c) ? CellState.Peg : CellState.Invalid;
                }
            }

            int center = Size / 2;
            _cells[center, center] = CellState.Empty;
        }

        public bool IsInsideBounds(int r, int c)
        {
            return r >= 0 && r < Size && c >= 0 && c < Size;
        }

        public bool IsPlayablePosition(int r, int c)
        {
            if (!IsInsideBounds(r, c))
                return false;

            int center = Size / 2;

            switch (Type)
            {
                case BoardType.English:
                    int arm = Size / 3;
                    bool rowInMiddleBand = r >= arm && r < Size - arm;
                    bool colInMiddleBand = c >= arm && c < Size - arm;
                    return rowInMiddleBand || colInMiddleBand;

                case BoardType.Diamond:
                    return Math.Abs(r - center) + Math.Abs(c - center) <= center;

                case BoardType.Hexagon:
                    return Math.Abs(r - c) <= center;

                default:
                    return false;
            }
        }

        public CellState GetCell(int r, int c)
        {
            if (!IsInsideBounds(r, c))
                throw new ArgumentOutOfRangeException(nameof(r), "Cell coordinates are outside the board.");

            return _cells[r, c];
        }

        public void SetCell(int r, int c, CellState value)
        {
            if (!IsInsideBounds(r, c))
                throw new ArgumentOutOfRangeException(nameof(r), "Cell coordinates are outside the board.");

            _cells[r, c] = value;
        }

        public int CountPegs()
        {
            int count = 0;

            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (_cells[r, c] == CellState.Peg)
                        count++;
                }
            }

            return count;
        }

        public string SerializeState()
        {
            var sb = new StringBuilder();

            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    sb.Append(_cells[r, c] switch
                    {
                        CellState.Peg => 'P',
                        CellState.Empty => 'E',
                        _ => 'I'
                    });
                }

                if (r < Size - 1)
                    sb.Append('/');
            }

            return sb.ToString();
        }

        public void LoadState(string serializedState)
        {
            if (string.IsNullOrWhiteSpace(serializedState))
                throw new ArgumentException("Serialized board state cannot be empty.");

            string[] rows = serializedState.Split('/');

            if (rows.Length != Size)
                throw new InvalidOperationException("Serialized state row count does not match board size.");

            for (int r = 0; r < Size; r++)
            {
                if (rows[r].Length != Size)
                    throw new InvalidOperationException("Serialized state column count does not match board size.");

                for (int c = 0; c < Size; c++)
                {
                    _cells[r, c] = rows[r][c] switch
                    {
                        'P' => CellState.Peg,
                        'E' => CellState.Empty,
                        'I' => CellState.Invalid,
                        _ => throw new InvalidOperationException("Invalid board state character found.")
                    };
                }
            }
        }
    }
}
