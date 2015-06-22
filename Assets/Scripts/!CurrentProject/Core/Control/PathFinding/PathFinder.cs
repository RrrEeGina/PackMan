using System.Collections.Generic;
using UnityEngine;

namespace Pacman
{
    public class PathFinder
    {
        #region Definitions
        private const int UNDEFINED_MARK = 0;
        private const int BEGIN_MARK = 1;
        private const int END_MARK = -1;
        private const int OBSTACLE_MARK = -2;
        private const int OUT_OF_RANGE_MARK = -3;
        private const int MARK_INCREMENT = 1;
        private const int COL_MULTIPLY = 1000;

        private PFCell[] beCells;
        private PFCell[,] grid;

        private int[] ambit = { 0, 1, 1, 0, 0, -1, -1, 0 };

        private List<PFCell> findingPath = new List<PFCell>();
        private Dictionary<int, PFCell> gaps;
        private List<PFCell> pasibilityCells = new List<PFCell>();

        #endregion
        #region Properties
        private PFCell this[Cell cell] { get { return grid[cell.Col, cell.Row]; } }
        #endregion
        #region Constructor
        public PathFinder()
        {
            Dictionary<int, PFCell> tempGaps = null;
            grid = new PFCell[GameMap.I.ColCount, GameMap.I.RowCount];
            var protoMX = GameMap.I.ProtoMX;
            for (int i = 0; i < grid.GetLength(0); i++)
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    var cell = new PFCell(GetPathFindActualCellType(protoMX[i, j]), i, j);

                    if (cell.Type != CellType.Wall)
                    {
                        pasibilityCells.Add(cell);
                        if (cell.Type == CellType.Gap)
                        {
                            if (tempGaps == null) tempGaps = new Dictionary<int, PFCell>();
                            tempGaps.Add(i * COL_MULTIPLY + j, cell);
                        }
                    }

                    grid[i, j] = cell;
                }

            if (tempGaps != null) ReverseGapKeys(tempGaps);
        }

        #endregion
        #region Methods
        #region Public
        public Vector2 GetRandomPoint()
        {
            return pasibilityCells[UnityEngine.Random.Range(0, pasibilityCells.Count)].Center;
        }

        public List<Vector2> RunFinding(Cell beginCell, Cell endCell)
        {
            beCells = new PFCell[] { this[beginCell], this[endCell] };
            Run();
            return GetPath();
        }
        #endregion
        #region Private
        private List<Vector2> GetPath()
        {
            return findingPath.ConvertAll(cell => cell.Center);
        }

        private bool Run()
        {
                Clear();
                beCells[0].Mark = BEGIN_MARK;
                beCells[1].Mark = END_MARK;
                if (FindPath(new List<PFCell> { beCells[0] }))
                {
                    BuildPath(beCells[1]);
                    findingPath.Reverse();
                    return true;
                }
                else
                    return false;
        }

        private void Clear()
        {
            findingPath.Clear();
            pasibilityCells.ForEach((c) => c.Mark = 0);
        }

        private bool FindPath(List<PFCell> currCells)
        {
            List<PFCell> cellsAraund = new List<PFCell>();
            foreach (var currCell in currCells)
            {
                var currCol = currCell.Col;
                var currRow = currCell.Row;
                for (int i = 0; i < ambit.Length - 1; i += 2)
                {
                    var cellNear = GetCellNear(currCol + ambit[i], currRow + ambit[i + 1], currCell);
                    if (cellNear != null)
                        switch (cellNear.Mark)
                        {
                            case UNDEFINED_MARK:
                                cellNear.Mark = currCell.Mark + MARK_INCREMENT;
                                cellsAraund.Add(cellNear);
                                break;
                            case END_MARK:
                                {
                                    cellNear.Mark = currCell.Mark + MARK_INCREMENT;
                                    return true;
                                }
                        }
                }
            }

            if (cellsAraund.Count != 0)
                return FindPath(cellsAraund);
            else
                return false;
        }

        private void BuildPath(PFCell previusCell)
        {
            findingPath.Add(previusCell);
            var currCol = previusCell.Col;
            var currRow = previusCell.Row;

            for (int i = 0; i < ambit.Length - 1; i += 2)
            {
                var cellNear = GetCellNear(currCol + ambit[i], currRow + ambit[i + 1], previusCell);
                if (cellNear != null && cellNear.Mark == previusCell.Mark - 1)
                {
                    if (cellNear.Mark != BEGIN_MARK)
                        BuildPath(cellNear);
                    else
                        findingPath.Add(cellNear);
                    break;
                }
            }
        }

        private PFCell GetCellNear(int col, int row, PFCell currCell)
        {
            if (row >= grid.GetLength(1) || col >= grid.GetLength(0) || row < 0 || col < 0)
                if (currCell.Type == CellType.Gap)
                    return gaps[currCell.Col * COL_MULTIPLY + currCell.Row];
                else
                    return null;

            if (grid[col, row].Type == CellType.Wall)
                return null;
            else
                return grid[col, row];
        }

        private CellType GetPathFindActualCellType(CellType preparationCellType)
        {

            switch (preparationCellType)
            {
                case CellType.Abyss:
                case CellType.Wall:
                    return CellType.Wall;
                case CellType.Gap:
                    return CellType.Gap;
                default:
                    return CellType.Path;
            }
        }

        private void ReverseGapKeys(Dictionary<int, PFCell> tempGaps)
        {
            var keys = new int[2];
            int i = 0;
            foreach (var pair in tempGaps) { keys[i] = pair.Key; i++; }
            gaps = new Dictionary<int, PFCell>();
            gaps[keys[1]] = tempGaps[keys[0]];
            gaps[keys[0]] = tempGaps[keys[1]];
        }
        #endregion
        #endregion
    }
}
