using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProdToFerra2D.Internal;
using UnityEngine;

namespace ProdToFerra2D.Public
{
    public class Polygon2D
    {
        private List<Line2D> lines = new List<Line2D>();
        private Vector2[] mainPoints = new Vector2[2];

        public Polygon2D(Vector2[] mainPoints, List<CLine> lines)
        {
            this.mainPoints[0] = mainPoints[0];
            this.mainPoints[1] = mainPoints[1];

            this.lines = lines.ConvertAll(l => new Line2D(l.NPoints[0], l.NPoints[1], l.IsSeam));
        }

        public List<Line2D> Lines { get { return lines; } }

        public Vector2[] MainPoints { get { return mainPoints; } }

        public void SetCellWidthAndHeight(float cellWidth, float cellHeight)
        {
            lines.ForEach((l) => l.SetCellWidthAndHeight(cellWidth, cellHeight));
        }
    }
}
