using System;
using UnityEngine;

namespace Pacman
{
    public class NonePlayerMovementHandler : IMovementHandler
    {
        #region Definition
        private GameMap map;
        private float size;
        private float cellSize;
        private Cell[] containingCells;

        private Vector2 currPosition;
        private Vector2 waitingDirection;
        private bool jumpRequired;
        private Action<Cell[]> setCells;

        private Action<Vector2, Vector2, bool> translate;

        #endregion
        #region Contructor
        public NonePlayerMovementHandler(float size, Action<Cell[]> setCells, Action<Vector2, Vector2, bool> translate)
        {
            this.size = size;
            this.setCells = setCells;
            this.translate = translate;

            cellSize = GameConst.UNITS_PER_CELL;
            map = GameMap.I;
        }
        #endregion
        #region Methods
        #region Public
        public void OnPositonChanged(Vector2 currPosition)
        {
            this.currPosition = currPosition;
            containingCells = GetOccupiedCell(currPosition, size);
            setCells(containingCells);
        }

        public void ChangeDirection(ref Vector2 direction, int dx, int dy)
        {
            direction.x = dx;
            direction.y = dy;
        }

        public void MoveRequest(ref Vector2 direction, float dt)
        {
            Vector2 cellAxis = GetCellAxis((int)direction.x);
            var dv = direction * dt;
            translate(dv, cellAxis, jumpRequired);
        }

        #endregion
        #region Private

        private Cell[] GetOccupiedCell(Vector2 point, float size)
        {
            var cell = map.GetCellByPoint(point);
            GapCell gap = cell.Type == CellType.Gap ? (GapCell)cell : null;

            var offsetX = point.x - (cell.Col + 0.5f) * cellSize;
            var offsetY = point.y - (cell.Row + 0.5f) * cellSize;

            if (Mathf.Abs(offsetX + offsetY) <= GameConst.ACCURACY)
            {
                jumpRequired = false;
                return new Cell[] { cell };
            }
            else if (Mathf.Abs(offsetX) <= GameConst.ACCURACY)
            {
                Cell cell2 = null;
                if (gap == null || offsetY * gap.Orientation.y <= 0)
                {
                    jumpRequired = false;
                    cell2 = map[cell.Col, cell.Row + (int)Mathf.Sign(offsetY)];
                }
                else
                {
                    jumpRequired = (1 - Mathf.Abs(offsetY / size) < GameConst.JUMP_RATIO_LIMIT);
                    cell2 = gap.Neighbor;
                }
                return new Cell[] { cell, cell2 };
            }
            else
            {
                Cell cell2 = null;
                if (gap == null || offsetX * gap.Orientation.x <= 0)
                {
                    jumpRequired = false;
                    cell2 = map[cell.Col + (int)Mathf.Sign(offsetX), cell.Row];
                }
                else
                {
                    jumpRequired = (1 - Mathf.Abs(offsetX / size) < GameConst.JUMP_RATIO_LIMIT);
                    cell2 = gap.Neighbor;
                }
                return new Cell[] { cell, cell2 };
            }

        }

        private Vector2 GetCellAxis(int dx)
        {
            if (dx == 0)
                return new Vector2(containingCells[0].Center.x, 0);
            else
                return new Vector2(0, containingCells[0].Center.y);
        }
        #endregion
        #endregion
    }
}

