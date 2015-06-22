using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProdToFerra2D.Internal
{
    public class CLine
    {
        #region Definitions
        protected Vector2[] p = new Vector2[2];
        protected bool isReversed = false;
        protected bool isSeam;

        internal int x1;
        internal int y1;
        internal int x2;
        internal int y2;

        internal CPolygon container;
        internal ST sideType;

        #endregion
        #region Constructor

        internal CLine(Vector2 p1, Vector2 p2, bool isSeam = false)
        {
            p = new Vector2[2];
            x1 = (int)p1.x;
            y1 = (int)p1.y;
            x2 = (int)p2.x;
            y2 = (int)p2.y;
            p[0] = p1;
            p[1] = p2;
            this.isSeam = isSeam;
        }

        internal CLine(int x, int y, ST sideType, bool isSeam = false)
        {
            this.sideType = sideType;
            switch (this.sideType)
            {
                case ST.Left:
                    x1 = x;
                    y1 = y;
                    x2 = x;
                    y2 = y + 1;
                    break;
                case ST.Top:
                    x1 = x;
                    y1 = y + 1;
                    x2 = x + 1;
                    y2 = y + 1;
                    break;
                case ST.Right:
                    x1 = x + 1;
                    y1 = y + 1;
                    x2 = x + 1;
                    y2 = y;
                    break;
                case ST.Down:
                    x1 = x + 1;
                    y1 = y;
                    x2 = x;
                    y2 = y;
                    break;
            }

            p[0] = new Vector2(x1, y1);
            p[1] = new Vector2(x2, y2);

            this.isSeam = isSeam;
        }
        #endregion
        #region Properties
        public bool IsSeam { get { return isSeam; } }

        public Vector2[] NPoints { get { return p; } }
        #endregion
        #region Methods
        public virtual void Reverse()
        {
            isReversed = !isReversed;
            var tx = x1;
            var ty = y1;
            x1 = x2;
            y1 = y2;
            x2 = tx;
            y2 = ty;

            switch (sideType)
            {
                case ST.Top:
                    sideType = ST.Down;
                    break;
                case ST.Right:
                    sideType = ST.Left;
                    break;
                case ST.Down:
                    sideType = ST.Top;
                    break;
                case ST.Left:
                    sideType = ST.Right;
                    break;
            }

            p.Reverse();
        }

        public virtual bool IsOrthogonal(CLine line)
        {
            return x1 != line.x2 && y2 == line.y1;
        }

        public virtual CornerType GetCornerType(CLine line)
        {
            if (this.sideType == line.sideType) return CornerType.None;

            CLine l1 = line;
            CLine l2 = this;

            if ((l1.sideType == ST.Left && l2.sideType == ST.Top) ||
                (l1.sideType == ST.Down && l2.sideType == ST.Left) ||
                (l1.sideType == ST.Right && l2.sideType == ST.Down) ||
                (line.sideType == ST.Top && l2.sideType == ST.Right))
                return CornerType.Open;

            return CornerType.Closed;
        }

        public override string ToString()
        {
            return "x1=" + x1 + " y1=" + y1 + " x2=" + x2 + " y1=" + y2;
        }
        #endregion
    }
}
