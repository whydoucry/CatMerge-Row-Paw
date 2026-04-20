using Microsoft.Xna.Framework;

namespace CatMergeRowPaw.Controllers
{
    public class GameController
    {
        public Match3Controller Match3 { get; }
        public MergeController Merge { get; }
        public GameMode CurrentMode { get; private set; }
        public Point? SelectedMatchCell { get; private set; }
        public Point? SelectedMergeCell { get; private set; }
        public bool HasCatReward { get; private set; }
        public bool IsPaused { get; set; }
        public bool StretchToFill = true;

        private bool _rewardGranted;

        public GameController()
        {
            Match3 = new Match3Controller();
            Merge = new MergeController();
            CurrentMode = GameMode.Menu;
        }

        public void Initialize()
        {
            Match3.Initialize();
            Merge.Initialize();
            AddStartingCats();
            HasCatReward = false;
            _rewardGranted = false;
        }

        public void SwitchMode()
        {
            if (CurrentMode == GameMode.Match3)
            {
                CurrentMode = GameMode.Merge;
            }
            else if (CurrentMode == GameMode.Merge)
            {
                CurrentMode = GameMode.Match3;
            }
            else
            {
                CurrentMode = GameMode.Menu;
            }

            ClearSelection();
        }

        public void Update(GameTime gameTime)
        {
            Match3.Update(gameTime);
            if (Match3.IsLevelComplete() && !_rewardGranted)
            {
                if (Merge.AddCat(new Cat(1)))
                {
                    HasCatReward = true;
                    _rewardGranted = true;
                }
            }
        }

        public void HandleClick(Point cell)
        {
            if (CurrentMode == GameMode.Match3)
            {
                HandleMatchClick(cell);
            }
            else if (CurrentMode == GameMode.Merge)
            {
                HandleMergeClick(cell);
            }
        }

        public void GoToMatch3()
        {
            if (Match3.IsLevelComplete())
            {
                Match3.PrepareNextLevel();
                HasCatReward = false;
                _rewardGranted = false;
            }

            CurrentMode = GameMode.Match3;
            ClearSelection();
        }

        public void GoToMerge()
        {
            CurrentMode = GameMode.Merge;
            ClearSelection();
        }

        public void GoToMenu()
        {
            CurrentMode = GameMode.Menu;
            ClearSelection();
        }

        public void GoToSettings()
        {
            CurrentMode = GameMode.Settings;
        }

        private void HandleMatchClick(Point cell)
        {
            if (Match3.IsLevelComplete())
            {
                return;
            }

            if (SelectedMatchCell == null)
            {
                SelectedMatchCell = cell;
                return;
            }

            if (AreAdjacent(SelectedMatchCell.Value, cell))
            {
                Match3.TrySwap(SelectedMatchCell.Value, cell);
            }

            SelectedMatchCell = null;
        }

        private void HandleMergeClick(Point cell)
        {
            if (SelectedMergeCell == null)
            {
                if (Merge.Board.Cats[cell.X, cell.Y] != null)
                {
                    SelectedMergeCell = cell;
                }
                return;
            }

            if (SelectedMergeCell.Value != cell)
            {
                Merge.TryMerge(SelectedMergeCell.Value, cell);
            }

            SelectedMergeCell = null;
        }

        private static bool AreAdjacent(Point a, Point b)
        {
            var dx = System.Math.Abs(a.X - b.X);
            var dy = System.Math.Abs(a.Y - b.Y);
            return dx + dy == 1;
        }

        public void ClearSelection()
        {
            SelectedMatchCell = null;
            SelectedMergeCell = null;
        }

        private void AddStartingCats()
        {
            for (var i = 0; i < 2; i++)
            {
                Merge.AddCat(new Cat(1));
            }
        }
    }
}
