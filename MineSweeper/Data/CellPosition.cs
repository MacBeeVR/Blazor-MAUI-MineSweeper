using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Data
{
    public class CellPosition
    {
        public readonly int X;
        public readonly int Y;

        public CellPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(CellPosition left, CellPosition right)
            => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(CellPosition left, CellPosition right)
            => left.X != right.X || left.Y != right.Y;

        public override bool Equals(object? obj)
            => obj is CellPosition ? this == (CellPosition)obj : false;

        public override int GetHashCode() 
            => (X * Y).GetHashCode();
    }
}
