using UnityEngine;

namespace Pacman
{

    public class GapCell : Cell
    {
        private Vector2 orientation;
        private GapCell neighbor;

        public GapCell Neighbor { get { return neighbor; } }
        public Vector2 Orientation { get { return orientation; } }

        public GapCell(ContentType content, int col, int row)
            : base(CellType.Gap, content, col, row)
        {
        }

        public void SetOrientation(Vector2 orientation)
        {
            this.orientation = orientation;
        }

        public void SetNeighbor(GapCell neighbor)
        {
            this.neighbor = neighbor;
            neighbor.neighbor = this;
        }

    }
}
