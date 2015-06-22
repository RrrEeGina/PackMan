using System;
using System.Collections.Generic;

namespace ProdToFerra2D.Internal
{
    class CCell : ICell
    {
        #region Definition
        private CellType type;

        internal List<CLine> lines = new List<CLine>();
        internal int sideMask;

        internal int x;
        internal int y;
        internal bool flag = true;
        #endregion
        #region Constructor
        internal CCell(int x, int y, CellType cellType)
        {
            this.x = x;
            this.y = y;
            this.type = cellType;
        }
        #endregion
        #region Properties
        public CellType Type { get { return type; } }

        public int X { get { return x; } }

        public int Y { get { return y; } }
        #endregion
        #region Methods
        internal bool IsWall
        {
            get
            {
                return type == CellType.Wall;
            }
        }

        internal void AddSide(byte sideN)
        {
            int n = 1 << sideN;
            sideMask |= n;

            if (sideN % 2 == 0)
            {
                lines.Add(new CLine(x, y, (ST)n));
            }
        }

        internal bool IsAdjacentWith(CCell cell)
        {
            return Math.Abs(x - cell.x) <= 1 && Math.Abs(y - cell.y) <= 1;
        }
        #endregion
    }
}
