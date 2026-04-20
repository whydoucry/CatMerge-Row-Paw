using Microsoft.Xna.Framework;

namespace CatMergeRowPaw.Controllers
{
    public class MergeController
    {
        public Board Board { get; }
        private readonly MergeSystem _mergeSystem;

        public MergeController()
        {
            Board = new Board(10, 10);
            _mergeSystem = new MergeSystem();
        }

        public void Initialize()
        {
            _mergeSystem.InitializeMergeBoard(Board);
        }

        public bool AddCat(Cat cat)
        {
            return _mergeSystem.AddCat(Board, cat);
        }

        public bool TryMerge(Point source, Point target)
        {
            return _mergeSystem.TryMerge(Board, source, target);
        }
    }
}
