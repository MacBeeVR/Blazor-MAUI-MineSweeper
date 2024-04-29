using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Data
{
    public interface IGameManager
    {
        bool Initialized    { get; }
        bool GameWon        { get; }
        bool GameOver       { get; }

        void InitializeGame(int gridSize, int minePercentage);
        Task ResetGameStateAsync();
        CellData? GetCell(int x, int y);
        void SelectCell(CellData data);

        bool GetInitialized();
        bool GetGameWon();
        bool GetGameOver();
    }

    public class GameManager : IGameManager
    {
        #region Properties
        public int              SafeCellCount   { get; private set; } = 0;
        public int              SelectedCount   { get; private set; } = 0;
        public int              GridSize        { get; private set; } = 0;
        public int              MinePercentage  { get; private set; } = 0;
        public bool             GameOver        { get; private set; } = false;
        public bool             GameWon         { get; private set; } = false;
        public bool             Initialized     { get; private set; } = false;
        public List<CellData>   Grid            { get; private set; } = new List<CellData>();
        #endregion

        private HashSet<CellData> SelectedCells = new HashSet<CellData>();

        #region Game Initialization
        public void InitializeGame(int gridSize, int minePercentage)
        {
            if (!Initialized)
            {
                var random      = new Random();
                var posDataMap  = new Dictionary<CellPosition, CellData>();
                var mineCount   = (gridSize * gridSize) * (minePercentage / 100.0);

                GridSize        = gridSize;
                MinePercentage  = minePercentage;
                SafeCellCount   = (GridSize * GridSize) - (int)mineCount;

                // Initialize Cell Coordinates and Data Object
                for (var y = 0; y < GridSize; y++)
                {
                    for (var x = 0; x < GridSize; x++)
                    {
                        var position = new CellPosition(x, y);
                        var cellData = new CellData(position);

                        Grid.Add(cellData);
                        posDataMap.Add(position, cellData);
                    }
                }

                // Distribute Mines
                for (var i = 0; i < mineCount; i++)
                {
                    Grid[random.Next(Grid.Count)].HasMine = true;
                }

                // Count up the number of neighbor mines
                Parallel.For(0, Grid.Count, (i) =>
                {
                    var neighbors = new List<CellPosition>
                    {
                        new CellPosition(Grid[i].Position.X - 1 , Grid[i].Position.Y - 1),
                        new CellPosition(Grid[i].Position.X     , Grid[i].Position.Y - 1),
                        new CellPosition(Grid[i].Position.X + 1 , Grid[i].Position.Y - 1),
                        new CellPosition(Grid[i].Position.X - 1 , Grid[i].Position.Y    ),
                        new CellPosition(Grid[i].Position.X + 1 , Grid[i].Position.Y    ),
                        new CellPosition(Grid[i].Position.X - 1 , Grid[i].Position.Y + 1),
                        new CellPosition(Grid[i].Position.X     , Grid[i].Position.Y + 1),
                        new CellPosition(Grid[i].Position.X + 1 , Grid[i].Position.Y + 1)
                    };

                    var neighborMines = 0;
                    foreach (var position in neighbors)
                        if (isValidNeighbor(position) && posDataMap[position].HasMine)
                            neighborMines++;

                    Grid[i].NeighborMineCount = neighborMines;

                    bool isValidNeighbor(CellPosition position)
                        => position.X >= 0 && position.X < GridSize
                        && position.Y >= 0 && position.Y < GridSize;
                });

                Initialized = true;
            }
        }

        public async Task ResetGameStateAsync()
        {
            SelectedCount   = 0;
            SafeCellCount   = 0;
            Initialized     = false;
            GameOver        = false;
            Grid            = new List<CellData>();
            SelectedCells   = new HashSet<CellData>();

            await Task.Delay(1);

            InitializeGame(GridSize, MinePercentage);
        }
        #endregion

        #region Cell Retrieval and Selection
        public CellData? GetCell(int x, int y)
        {
            if (x < 0 || x >= GridSize || y < 0 || y >= GridSize)
                return null;

            return Grid[y * GridSize + x];
        }

        public void SelectCell(CellData cell)
        {
            if (!SelectedCells.Contains(cell))
            {
                SelectedCells.Add(cell);
                GameWon     = SelectedCells.Count == SafeCellCount;
                GameOver    = GameWon || cell.HasMine;
            }
        }
        #endregion

        #region Property Accessors
        public bool GetInitialized()    => Initialized;
        public bool GetGameWon()        => GameWon;
        public bool GetGameOver()       => GameOver;
        #endregion
    }
}
