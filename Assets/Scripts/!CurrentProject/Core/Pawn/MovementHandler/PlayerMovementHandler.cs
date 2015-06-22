using System;
using UnityEngine;

namespace Pacman
{
    public class PlayerMovementHandler : IMovementHandler
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
        public PlayerMovementHandler(float size, Action<Cell[]> setCells, Action<Vector2, Vector2, bool> translate)
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
            if (dx == 0 && dy == 0) return;

            if (dx * dy != 0)
            {

                bool canGoX = CheckPassability(dx, 0, direction);
                bool canGoY = CheckPassability(0, dy, direction);

                if ((direction.x == 0 || !canGoY))
                {
                    if (canGoX)
                    {
                        direction.x = dx;
                        direction.y = 0;
                    }
                    waitingDirection.x = dx;
                    waitingDirection.y = 0;
                }
                else if ((direction.y == 0 || !canGoX))
                {
                    if (canGoY)
                    {
                        direction.x = 0;
                        direction.y = dy;
                    }
                    waitingDirection.x = 0;
                    waitingDirection.y = dy;
                }
            }
            else if (direction.x != dx || direction.y != dy)
            {
                if (CheckPassability(dx, dy, direction))
                {
                    direction.x = dx;
                    direction.y = dy;
                }
                waitingDirection.x = dx;
                waitingDirection.y = dy;
            }
        }

        public void MoveRequest(ref Vector2 direction, float dt)
        {
            //dt = dt < GameConst.CRITICAL_DELTA_TIME ? Time.fixedDeltaTime : GameConst.CRITICAL_DELTA_TIME;
            var dv = direction * dt;

            Vector2 cellAxis;
            bool waitingDirectionSuccess = false;

            if (waitingDirection != direction && CheckPassabilityAndGetCellAxis(waitingDirection.x, waitingDirection.y, direction, out cellAxis))
            {
                translate(dv, cellAxis, jumpRequired);
                waitingDirectionSuccess = true;
                direction = waitingDirection;
            }

            if (!waitingDirectionSuccess && CheckPassabilityAndGetCellAxis((int)direction.x, (int)direction.y, out cellAxis))
                translate(dv, cellAxis, jumpRequired);
        }

        #endregion
        #region Private
        private bool CheckPassabilityAndGetCellAxis(float dx, float dy, Vector2 direction, out Vector2 cellAxis)
        {
            return CheckPassabilityAndGetCellAxis((int)dx, (int)dy, direction, out cellAxis);
        }

        private bool CheckPassabilityAndGetCellAxis(int dx, int dy, Vector2 direction, out Vector2 cellAxis)
        {
            cellAxis = new Vector2();

            if (containingCells.Length > 1)
            {
                if (direction.x != 0 && dy != 0)
                    foreach (var cell in containingCells)
                        if (map[cell.Col, cell.Row + dy].Type == CellType.Wall)
                            return false;

                if (direction.y != 0 && dx != 0)
                    foreach (var cell in containingCells)
                        if (map[cell.Col + dx, cell.Row].Type == CellType.Wall)
                            return false;

                cellAxis = GetCellAxis(dx);
            }
            else
                if (GetCellNear(containingCells[0], dx, dy).Type == CellType.Wall)
                    return false;
                else
                    cellAxis = GetCellAxis(dx);

            return true;
        }

        private bool CheckPassabilityAndGetCellAxis(int dx, int dy, out Vector2 cellAxis)
        {
            cellAxis = new Vector2();
            if (containingCells.Length == 1)
                if (GetCellNear(containingCells[0], dx, dy).Type == CellType.Wall)
                    return false;

            cellAxis = GetCellAxis(dx);
            return true;
        }

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

        private bool CheckPassability(int dx, int dy, Vector2 direction)
        {
            if (containingCells.Length > 1)
            {
                if (direction.x != 0 && dy != 0)
                    foreach (var cell in containingCells)
                        if (map[cell.Col, cell.Row + dy].Type == CellType.Wall)
                            return false;

                if (direction.y != 0 && dx != 0)
                    foreach (var cell in containingCells)
                        if (map[cell.Col + dx, cell.Row].Type == CellType.Wall)
                            return false;
            }
            else
                if (GetCellNear(containingCells[0], dx, dy).Type == CellType.Wall)
                    return false;

            return true;
        }

        private Vector2 GetCellAxis(int dx)
        {
            if (dx == 0)
                return new Vector2(containingCells[0].Center.x, 0);
            else
                return new Vector2(0, containingCells[0].Center.y);
        }

        private Cell GetCellNear(Cell cell, int dx, int dy)
        {
            GapCell gap = cell.Type == CellType.Gap ? (GapCell)cell : null;
            if (gap != null)
            {
                if (gap.Orientation.x * dx == 1) return gap.Neighbor;
                if (gap.Orientation.y * dy == 1) return gap.Neighbor;
            }

            return map[cell.Col + dx, cell.Row + dy];
        }
        #endregion
        #endregion
    }
}
