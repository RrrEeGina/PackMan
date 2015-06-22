using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ProdToFerra2D.Internal;

namespace ProdToFerra2D.Public
{
    public class Line2D : CLine
    {
        #region Definitions
        private Vector2[] realPoints = new Vector2[2];
        #endregion
        #region Constructor
        public Line2D(Vector2 unitP1, Vector2 unitP2, bool isSeam = false)
            : base(unitP1, unitP2, isSeam)
        {
            realPoints[0].x = p[0].x;
            realPoints[0].y = p[0].y;
            realPoints[1].x = p[1].x;
            realPoints[1].y = p[1].y;
        }
        #endregion
        #region Properties
        public Vector2[] Points { get { return realPoints; } }
        #endregion
        #region Methods
        public override void Reverse()
        {
            base.Reverse();
            realPoints.Reverse();
        }

        public void SetCellWidthAndHeight(float cellWidth, float cellHeight)
        {
            realPoints[0].x = cellWidth * p[0].x;
            realPoints[0].y = cellHeight * p[0].y;
            realPoints[1].x = cellWidth * p[1].x;
            realPoints[1].y = cellHeight * p[1].y;
        }

        #endregion
    }
}
