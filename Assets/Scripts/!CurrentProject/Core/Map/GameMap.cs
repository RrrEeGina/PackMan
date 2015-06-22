using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pacman
{
    public class GameMap : IEnumerable<Cell>, IJsonSerializable
    {
        #region Static
        private static GameMap _i;
        public static GameMap I
        {
            get
            {
                return _i;
            }
        }
        #endregion
        #region Definition
        private CellType[,] protoMX;

        private Cell[,] mx;
        private List<GapCell> gapCells;

        private int rowCount;
        private int colCount;
        private float cellSize;

        private Vector2 singlePlayerRespawn;
        private List<Vector2> multyPlayerRespawans = new List<Vector2>();
        private List<Vector2> npcRespawns = new List<Vector2>();

        private int eatCount;

        private Action onContentIsOver;
        #endregion
        #region Properties
        public int RowCount { get { return rowCount; } }

        public int ColCount { get { return colCount; } }

        public Vector2 SinglePlayerRespawn { get { return singlePlayerRespawn; } }
        public List<Vector2> MultyPlayerRespawans { get { return multyPlayerRespawans; } }
        public List<Vector2> NPCRespawns { get { return npcRespawns; } }

        public Cell this[int col, int row]
        {
            get
            {
                return mx[col, row];
            }
        }

        public CellType[,] ProtoMX { get { return protoMX; } }
        #endregion
        #region Constructor
        public GameMap(CellType[,] mxCell, ContentType[,] mxContent)
        {
            GameManager.I.OnObjectForSaveInstantiated(this);

            protoMX = mxCell;
            colCount = mxCell.GetLength(0);
            rowCount = mxCell.GetLength(1);

            mx = new Cell[colCount, rowCount];
            for (int i = 0; i < colCount; i++)
                for (int j = 0; j < rowCount; j++)
                {
                    var cellType = GetRuntimeActualCellType(mxCell[i, j]);
                    var contentType = mxContent[i, j];

                    if (contentType == ContentType.Coockie || contentType == ContentType.Drug)
                        eatCount++;

                    if (cellType != CellType.Gap)
                        mx[i, j] = new Cell(cellType, mxContent[i, j], i, j);
                    else
                    {
                        var gapCell = new GapCell(mxContent[i, j], i, j);
                        mx[i, j] = gapCell;
                        if (gapCells == null) gapCells = new List<GapCell>();
                        gapCells.Add(gapCell);
                    }

                    DefineRespawns(mx[i, j], mxCell[i, j]);
                }

            if (gapCells != null) DefineGapCells();

            cellSize = GameConst.UNITS_PER_CELL;
            _i = this;
        }
        #endregion
        #region Methods
        #region Public
        public void SubscribeOnContentIsOver(Action onContentIsOver)
        {
            this.onContentIsOver = onContentIsOver;
        }

        public Cell GetCellByPoint(Vector2 point)
        {
            var col = Mathf.FloorToInt(point.x / cellSize);
            var row = Mathf.FloorToInt(point.y / cellSize);
            return mx[col, row];
        }
        #endregion
        #region Private
        private void DefineRespawns(Cell cell, CellType buildCellType)
        {
            switch (buildCellType)
            {
                case CellType.PlayerSingleRespawn:
                    singlePlayerRespawn = cell.Center;
                    break;
                case CellType.PlayerMultyRespawn:
                    multyPlayerRespawans.Add(cell.Center);
                    break;
                case CellType.NPCRespawn:
                    npcRespawns.Add(cell.Center);
                    break;
            }
        }

        private void DefineGapCells()
        {
            if (gapCells.Count == 2)
            {
                foreach (var cell in gapCells)
                {
                    if (cell.Row + 1 < rowCount && mx[cell.Col, cell.Row + 1].Type == CellType.Path) cell.SetOrientation(new Vector2(0, -1));
                    else if (cell.Col + 1 < colCount && mx[cell.Col + 1, cell.Row].Type == CellType.Path) cell.SetOrientation(new Vector2(-1, 0));
                    else if (cell.Row - 1 >= 0 && mx[cell.Col, cell.Row - 1].Type == CellType.Path) cell.SetOrientation(new Vector2(0, 1));
                    else if (cell.Col - 1 >= 0 && mx[cell.Col - 1, cell.Row].Type == CellType.Path) cell.SetOrientation(new Vector2(1, 0));
                }
                gapCells[0].SetNeighbor(gapCells[1]);
            }
            else
                Debug.LogError("Gap Cells count not equals 2");

        }

        private CellType GetRuntimeActualCellType(CellType preparationCellType)
        {

            switch (preparationCellType)
            {
                case CellType.Wall:
                case CellType.SpookDoor:
                    return CellType.Wall;
                case CellType.Gap:
                    return CellType.Gap;
                default:
                    return CellType.Path;
            }
        }
        #endregion
        #region IEnumerable
        public IEnumerator<Cell> GetEnumerator()
        {
            foreach (var cell in mx)
                yield return cell;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
        #region Json
        public JsonObjectType JsonType { get { return JsonObjectType.Map; } }

        public IJsonObject GetJsonObject()
        {
            JsonMap map = new JsonMap();
            map.ContentLines = new List<string>();
            for (int i = rowCount - 1; i >= 0; i--)
            {
                var line = "";
                for (int j = 0; j < colCount; j++)
                    line += ((byte)this[j, i].Content).ToString();
                map.ContentLines.Add(line);
            }
            map.Cherry = (JsonCherry)GameObject.FindObjectOfType<Cherry>().GetJsonObject();
            map.DrugAffect = (JsonDrugAffect)DrugAffector.I.GetJsonObject();
            return map;
        }

        #endregion
        #endregion
        #region CallBack
        public void OnContentChanged(Content content, bool isDestroyed)
        {
            var cell = GetCellByPoint(content.T.position);
            if (isDestroyed)
            {
                cell.SetContent(ContentType.Empty);
                eatCount--;
                if (eatCount <= 0) onContentIsOver();
            }
            else
                cell.SetContent(content.Type);
        }
        #endregion
    }
}
