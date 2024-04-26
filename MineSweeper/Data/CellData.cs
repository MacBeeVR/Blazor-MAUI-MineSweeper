using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Data
{
    public class CellData
    {
        public readonly CellPosition Position;
        
        public bool HasMine             { get; set; }
        public int  NeighborMineCount   { get; set; }
        
        public CellData(CellPosition position)
        {
            Position = position;
        }
    }
}
