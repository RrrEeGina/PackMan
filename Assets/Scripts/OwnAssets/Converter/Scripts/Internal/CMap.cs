using System.Collections.Generic;
using ProdToFerra2D.Public;


namespace ProdToFerra2D.Internal
{
    /// <summary>
    /// This class makes initial representation of data matrix and
    /// also it is the entry point of analysis algorithm.
    /// </summary>
    class CMap : ICellContainer
    {
        #region Definition
        private CCell[,] mx;

        private int colCount;
        private int rowCount;

        private CCell entranceCell;
        private CCell exitCell;
        private List<CCell> doorCells = new List<CCell>();

        private List<CPolygon> finalPolygons = new List<CPolygon>();

        private bool isDiagonal = false;

        internal List<CCell> wall = new List<CCell>();
        internal List<CRegion> regions = new List<CRegion>();
        
        #endregion
        #region Constructor
        //internal CMap(MapRepresent_ProD representMap)
        //{
        //    colCount = representMap.ColCount;
        //    rowCount = representMap.RowCount;
        //    mx = new CCell[colCount, rowCount];

        //    for (int i = 0; i < colCount; i++)
        //        for (int j = 0; j < rowCount; j++)
        //        {
        //            var cell = new CCell(i, j, representMap[i, j]);

        //            mx[i, j] = cell;
        //            if (cell.IsWall) wall.Add(cell);
        //            else
        //                switch (cell.Type)
        //                {
        //                    case CellType.Entrance:
        //                        entranceCell = cell;
        //                        break;
        //                    case CellType.Exit:
        //                        exitCell = cell;
        //                        break;
        //                    case CellType.Door:
        //                        doorCells.Add(cell);
        //                        break;
        //                }
        //        }
        //}

        internal CMap(CellType[,] cellMatrix)
        {
            colCount = cellMatrix.GetLength(0);
            rowCount = cellMatrix.GetLength(1);
            mx = new CCell[colCount, rowCount];

            for (int i = 0; i < colCount; i++)
                for (int j = 0; j < rowCount; j++)
                {
                    var cell = new CCell(i, j, cellMatrix[i, j]);

                    mx[i, j] = cell;
                    if (cell.IsWall) wall.Add(cell);
                    else
                        switch (cell.Type)
                        {
                            case CellType.Entrance:
                                entranceCell = cell;
                                break;
                            case CellType.Exit:
                                exitCell = cell;
                                break;
                            case CellType.Door:
                                doorCells.Add(cell);
                                break;
                        }
                }
        }

        #endregion
        #region Properties
        internal int RowCount { get { return rowCount; } }
        internal int ColCount { get { return colCount; } }
        internal CCell EntranceCell { get { return entranceCell; } }
        internal CCell ExitCell { get { return exitCell; } }
        internal List<CCell> DoorCells { get { return doorCells; } }
        internal List<CPolygon> FinalPolygons { get { return finalPolygons; } }
        public CCell this[int x, int y]
        {
            get
            {
                if (x >= 0 && x < colCount && y >= 0 && y < rowCount)
                    return mx[x, y];
                else
                    return null;
            }
        }
        #endregion
        #region Methods
        #region Analytics
        internal void RunAnalyze()
        {
            BuildRegions();
            regions.ForEach((r) => { finalPolygons.Add(r.RunAnalize()); });
        }

        private void BuildRegions()
        {
            var twall = new List<CCell>(wall);

            while (twall.Count > 0)
            {
                List<CCell> cells = new List<CCell>();

                FindAdjacentCell(twall[0], cells);

                if (cells.Count > 0) regions.Add(new CRegion(cells, colCount, rowCount));
                twall.RemoveAll((c) => { return !c.flag; });
            }
        }

        private void FindAdjacentCell(CCell cell, List<CCell> adjancentCells)
        {
            var near = GetNear(cell);
            byte sideN = 1;

            foreach (var c in near)
            {
                if (c != null)
                {
                    if (c.flag)
                    {
                        c.flag = false;
                        adjancentCells.Add(c);
                        if (cell != c) FindAdjacentCell(c, adjancentCells);
                    }
                }
                else
                    cell.AddSide(sideN);

                sideN++;
            }
        }

        private CCell[,] GetNear(CCell cell)
        {
            var cells = new CCell[3, 3];
            int x = cell.x;
            int y = cell.y;

            int col = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                int row = 0;
                for (int j = y - 1; j <= y + 1; j++)
                {

                    if (i >= 0 && j >= 0 && i < colCount && j < rowCount)
                    {
                        var c = mx[i, j];
                        if (c.IsWall && (isDiagonal || col == 1 || row == 1))
                            cells[col, row] = c;
                    }
                    row++;
                }
                col++;
            }

            return cells;
        }

        #endregion
        #region ICellContainer

        private List<ICell> _cells;

        public IEnumerable<ICell> GetCells()
        {
            foreach (var cell in mx)
                yield return cell;
        }

        
        public List<ICell> AllCells
        {
            get
            {
                if (_cells == null) _cells = new List<ICell>();
                foreach (var c in GetCells())
                    _cells.Add(c);

                return _cells;
            }
        }

        public ContainerType Type { get { return ContainerType.Map; } }
        #endregion
        #endregion
    }
}
