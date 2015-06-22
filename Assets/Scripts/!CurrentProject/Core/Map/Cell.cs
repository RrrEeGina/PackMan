using UnityEngine;

namespace Pacman
{
    public class Cell
    {
        #region Definition
        private CellType type;
        private ContentType content;
        private int row;
        private int col;
        private Vector2 center;
        #endregion
        #region Properties
        public CellType Type { get { return type; } }
        public ContentType Content { get { return content; } }
        public int Row { get { return row; } }
        public int Col { get { return col; } }
        public Vector2 Center { get { return center; } }
        #endregion
        #region Constructor
        public Cell(CellType type, ContentType content, int col, int row)
        {
            this.type = type;
            this.content = content;
            this.row = row;
            this.col = col;
            this.center = new Vector2((col + 0.5f) * GameConst.UNITS_PER_CELL, (row + 0.5f) * GameConst.UNITS_PER_CELL);
        }
        #endregion
        #region Methods
        public void SetContent(ContentType content)
        {
            this.content = content;
        }

        public override string ToString()
        {
            return Col + " " + Row;
        }

        #endregion
    }
}
