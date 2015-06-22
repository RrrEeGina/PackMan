using UnityEngine;

namespace Pacman
{
    public class PFCell
    {
        #region Definition
        private CellType type;
        private int row;
        private int col;
        private Vector2 center;
        #endregion
        #region Properties
        public CellType Type { get { return type; } }
        public int Row { get { return row; } }
        public int Col { get { return col; } }
        public Vector2 Center { get { return center; } }
        public int Mark { get; set; }
        #endregion
        #region Constructor
        public PFCell(CellType type, int col, int row)
        {
            this.type = type;
            this.row = row;
            this.col = col;
            this.center = new Vector2((col + 0.5f) * GameConst.UNITS_PER_CELL, (row + 0.5f) * GameConst.UNITS_PER_CELL);
        }
        #endregion
        #region Methods
        public override string ToString()
        {
            return Col + " " + Row;
        }
        #endregion
    }
}
