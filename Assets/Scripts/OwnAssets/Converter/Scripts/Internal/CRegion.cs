using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProdToFerra2D.Internal
{
    /// <summary>
    /// This class present second phase of analysis algorithm.
    /// There are 
    /// </summary>
    class CRegion : ICellContainer
    {
        #region Definition
        internal List<CCell> cells = new List<CCell>();
        
        internal List<CPolygon> polys = new List<CPolygon>();
        internal CPolygon mainPolygon;

        private List<CLine>[,] linesMX;
        private List<CLine>[,] linesMXCopy;

        private CLine firstLine;
        #endregion
        #region Constructor
        public CRegion(List<CCell> cells, int mapWidth, int mapHeight)
        {
            this.cells = cells;

            linesMX = new List<CLine>[mapWidth + 1, mapHeight + 1];
            linesMXCopy = new List<CLine>[mapWidth + 1, mapHeight + 1];

            cells.ForEach((c) =>
            {
                c.lines.ForEach((l) =>
                {
                    if (linesMX[l.x1, l.y1] == null)
                    {
                        linesMX[l.x1, l.y1] = new List<CLine>();
                        linesMX[l.x1, l.y1].Add(l);

                        linesMXCopy[l.x1, l.y1] = new List<CLine>();
                        linesMXCopy[l.x1, l.y1].Add(l);
                    }
                    else
                    {
                        linesMX[l.x1, l.y1].Add(l);
                        linesMXCopy[l.x1, l.y1].Add(l);
                    }

                    if (firstLine == null)
                    {
                        firstLine = l;
                        linesMX[l.x1, l.y1] = null;
                    }
                });
            });
        }
        #endregion
        #region Methods
        #region Analytics

        internal CPolygon RunAnalize()
        {
            PreBuilding();
            DefineSubPolygons();
            return mainPolygon.RunAnalize();
        }

        private void PreBuilding()
        {
            while (firstLine != null)
            {
                PreBuildPolygon(firstLine);

                firstLine = null;

                foreach (var l in linesMX)
                    if (l != null)
                    {
                        firstLine = l[0];
                        break;
                    }
            }
        }

        private void PreBuildPolygon(CLine fl)
        {
            var lines = new List<CLine>();
            Vector2[] mp = { new Vector2(fl.x1, fl.y1), new Vector2(fl.x2, fl.y2) };

            var polygon = new CPolygon(mp, lines);
            int diagonals = 0;

            while (fl != null)
            {
                lines.Add(fl);
                fl.container = polygon;

                foreach (var p in fl.NPoints)
                {
                    if (mp[0].x > p.x)
                        mp[0].x = p.x;

                    if (mp[0].y > p.y)
                        mp[0].y = p.y;

                    if (mp[1].x < p.x)
                        mp[1].x = p.x;

                    if (mp[1].y < p.y)
                        mp[1].y = p.y;
                }


                var nl = linesMX[fl.x2, fl.y2];

                if (nl != null && nl.Count != 0)
                {
                    if (nl.Count == 1)
                    {
                        fl = nl[0];
                        linesMX[fl.x1, fl.y1] = null;
                    }
                    else
                    {
                        diagonals++;
                        if (fl.IsOrthogonal(nl[0]))
                        {
                            fl = nl[0];
                            nl.RemoveAt(0);
                        }
                        else
                        {
                            fl = nl[1];
                            nl.RemoveAt(1);
                        }
                    }
                }
                else
                    fl = null;
            }

            polygon.diagonals = diagonals;
            polygon.Recalculate();
            polys.Add(polygon);
        }

        private void DefineSubPolygons()
        {
            bool freeExists = true;
            var freePolys = new List<CPolygon>(polys);
            while (freeExists)
            {
                var children = freePolys[0];
                freePolys.RemoveAt(0);
                
                CPolygon parent = null;

                foreach (var p in polys)
                    if (p.Contains(children) && (parent == null || p.recS < parent.recS))
                        parent = p;

                if (parent != null)
                {
                    children.depth = 1;
                    parent.subPolygons.Add(children);
                }
                else
                {
                    mainPolygon = children;
                    mainPolygon.depth = 0;
                }

                if (freePolys.Count == 0) freeExists = false;
            }

            mainPolygon.SetLinesMX(linesMXCopy);
            DefineSubPolygonsDepth(mainPolygon);
        }

        private void DefineSubPolygonsDepth(CPolygon poly)
        {
            foreach (var p in poly.subPolygons)
            {
                p.depth = poly.depth + 1;
                DefineSubPolygonsDepth(p);
            }

        }

        #endregion
        #region ICellContainer
        private List<ICell> _cells;

        public IEnumerable<ICell> GetCells()
        {
            foreach (var c in cells)
                yield return c;
        }

        public List<ICell> AllCells
        {
            get
            {
                if (_cells == null) _cells = cells.ConvertAll(c => (ICell)c);
                return _cells;
            }
        }

        public ContainerType Type { get { return ContainerType.Region; } }
        #endregion
        #endregion
    }
}
