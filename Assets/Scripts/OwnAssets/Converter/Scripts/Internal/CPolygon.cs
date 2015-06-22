using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProdToFerra2D.Internal
{
    internal class CPolygon
    {
        #region Definition
        private List<CLine>[,] mx;
        private int maxX;
        private int maxY;

        private bool isReversed = false;

        internal List<CLine> lines;
        internal Vector2[] mainPoints;

        private List<CLine> finalLines;
        private List<CPolygon> passedPolys;

        internal float recS;
        internal float perimeter;

        internal List<CPolygon> subPolygons = new List<CPolygon>();
        internal int depth = -1;

        internal int diagonals;
        #endregion
        #region Constructor
        internal CPolygon(Vector2[] mainPoints, List<CLine> lines)
        {
            this.mainPoints = mainPoints;
            this.lines = lines;
            this.recS = (mainPoints[1].x - mainPoints[0].x) * (mainPoints[1].y - mainPoints[0].y);
            this.perimeter = lines.Count;
        }
        #endregion
        #region Methods
        #region Analytics

        internal CPolygon RunAnalize()
        {
            finalLines = new List<CLine>();
            passedPolys = new List<CPolygon>();
            BuildEntirePolygon(lines[0]);
            return new CPolygon(new Vector2[] { mainPoints[0], mainPoints[1] }, finalLines);
        }

        private void BuildEntirePolygon(CLine entranceLine, CLine joinLine = null)
        {
            var poly = entranceLine.container;
            passedPolys.Add(poly);

            int n = poly.lines.FindIndex((line) => { return line == entranceLine; }); //Long :( need Dictionary!
            int previousN = n > 0 ? n - 1 : poly.lines.Count - 1;
            bool insideCast = poly.depth == 0;

            CLine l = entranceLine;
            CRayQualifier qualifier = new CRayQualifier(entranceLine, poly.lines[previousN], true, insideCast);

            bool needMorePolygons = true;
            while (needMorePolygons)
            {
                finalLines.Add(new CLine(l.NPoints[0], l.NPoints[1]));

                var rays = qualifier.GetRay();

                CLine foundLine = null;
                if (rays[0] != 0)
                    foundLine = HorizontalCast(l.x1, l.y1, rays[0]);
                if (rays[1] != 0)
                    foundLine = VerticalCast(l.x1, l.y1, rays[1]);

                if (foundLine != null && !passedPolys.Contains(foundLine.container))
                {
                    finalLines.Add(new CLine(l.NPoints[0], foundLine.NPoints[0], true));
                    BuildEntirePolygon(foundLine, finalLines.Last());
                    n = n - 1 >= 0 ? n - 1 : poly.lines.Count - 1;
                }


                if (l.NPoints[1] == entranceLine.NPoints[0])
                {
                    needMorePolygons = false;
                    if (joinLine != null)
                        finalLines.Add(new CLine(joinLine.NPoints[1], joinLine.NPoints[0], true));
                }
                else
                {
                    n = n + 1 < poly.lines.Count ? n + 1 : 0;
                    l = poly.lines[n];
                    qualifier.NextStep(l);
                }
            }
        }

        private CLine HorizontalCast(int x0, int y0, int dx)
        {
            for (int x = x0 + 1; x > 0 && x < maxX; x += dx)
                if (mx[x, y0] != null)
                {
                    if (mx[x, y0].Count == 1)
                        return mx[x, y0][0];
                    else
                        UnityEngine.Debug.LogWarning("Diagonal found by Horizontal Cast from" + new Vector2(x0, y0));
                }

            return null;
        }

        private CLine VerticalCast(int x0, int y0, int dy)
        {
            for (int y = y0 + 1; y > 0 && y < maxY; y += dy)
                if (mx[x0, y] != null)
                {
                    if (mx[x0, y].Count == 1)
                        return mx[x0, y][0];
                    else
                        UnityEngine.Debug.LogWarning("Diagonal found by Vertical Cast from" + new Vector2(x0, y0));
                }

            return null;
        }

        #endregion
        #region Misc

        internal void Recalculate()
        {
            this.recS = (mainPoints[1].x - mainPoints[0].x) * (mainPoints[1].y - mainPoints[0].y);
            this.perimeter = lines.Count;
        }

        private void Reverse()
        {
            isReversed = !isReversed;
            lines.ForEach((l) => { l.Reverse(); });
        }

        internal void SetLinesMX(List<CLine>[,] mx)
        {
            this.mx = mx;
            if (mx != null)
            {
                maxX = mx.GetLength(0);
                maxY = mx.GetLength(1);
            }
        }

        internal bool Contains(CPolygon poly)
        {
            return this.mainPoints[0].x < poly.mainPoints[0].x &&
                   this.mainPoints[0].y < poly.mainPoints[0].y &&
                   this.mainPoints[1].x > poly.mainPoints[1].x &&
                   this.mainPoints[1].y > poly.mainPoints[1].y;
        }

        public override string ToString()
        {
            return mainPoints[0] + " " + mainPoints[1];
        }
        #endregion
        #endregion
        #region Unused
        //private void Segmentation(CPolygon polygon, List<FerraLine> ferraLines, CLine firstLine, bool forward)
        //{
        //    bool needMorePolygons = true;

        //    bool insideCast = polygon.depth == 0;
        //    forward = polygon.depth == 0 && forward;

        //    int n = 0;
        //    CLine l = polygon.lines[n];
        //    CRayQualifier caster = new CRayQualifier(l, polygon.lines.Last(), forward, insideCast);

        //    while (needMorePolygons)
        //    {
        //        var ray = caster.GetRay();
        //        CLine line = null;

        //        if (ray[0] != 0)
        //            line = HorizontalCast(l.x1, l.y1, ray[0]);

        //        if (ray[1] != 0)
        //            line = VerticalCast(l.x1, l.y1, ray[1]);

        //        if (line != null && l.container != this)
        //        {
        //            List<FerraLine> flines = new List<FerraLine>();
        //            flines.Add(new FerraLine(l.NPoints[0], line.NPoints[0]));
        //            Segmentation(l.container, flines, line, true);
        //        }

        //        l.flag = false;

        //        n++;

        //        //if (n >= polygon.perimeter / 2 + 1)
        //        //{
        //        //    //Try cast to Main Polygon;
        //        //    //break;
        //        //}
        //        //else
        //        //{
        //        l = polygon.lines[n];
        //        caster.NextStep(l);
        //        //}
        //    }
        //}
        //private CLine TryCastToPolygon(CPolygon polygon, CLine line)
        //{
        //    return null;
        //}
        #endregion
    }
}
