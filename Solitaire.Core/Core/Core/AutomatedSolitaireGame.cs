using cs449sprint2.Models;

namespace cs449sprint2.Core
{
    public class AutomatedSolitaireGame : SolitaireGameBase
    {
        private readonly Random _random = new();

        public bool MakeAutomaticMove()
        {
            var moves = GetValidMoves();

            if (moves.Count == 0)
                return false;

            Move chosenMove = moves[_random.Next(moves.Count)];

            return MakeMove(
                chosenMove.FromRow,
                chosenMove.FromCol,
                chosenMove.ToRow,
                chosenMove.ToCol);
        }

        public void AutoPlayToEnd()
        {
            while (!IsGameOver())
            {
                if (!MakeAutomaticMove())
                    break;
            }
        }
    }
}
