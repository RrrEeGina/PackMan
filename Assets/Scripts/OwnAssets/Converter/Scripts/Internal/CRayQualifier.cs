using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProdToFerra2D.Internal
{
    class CRayQualifier
    {
        private CLine lastLine;
        private int[] ray = new int[2];

        private bool inCast;
        private bool forward;
        internal CRayQualifier(CLine firstLine, CLine endLine, bool forwardDirection, bool insideCast = true)
        {
            forward = forwardDirection;

            DefineRay(firstLine, endLine);

            lastLine = firstLine;
            inCast = insideCast;
        }

        internal int[] GetRay()
        {
            return ray;
        }

        internal void NextStep(CLine l)
        {
            DefineRay(l, lastLine);
            lastLine = l;
        }

        private void DefineRay(CLine l1, CLine l2)
        {

            //Debug.Log(l1 + " " + l1.sideType + " / " + l2 + " " + l2.sideType);

            if (forward)
                DefineRay(l1.sideType, l1.GetCornerType(l2));
            else
                DefineRay(l2.sideType, l2.GetCornerType(l1));
        }

        private void DefineRay(ST sideType, CornerType corner)
        {
            int k = 0;

            //Debug.Log("Corner is " + corner);

            switch (corner)
            {
                case CornerType.Closed:
                    ray[0] = 0;
                    ray[1] = 0;
                    return;
                case CornerType.None:
                    k = 0;
                    break;
                case CornerType.Open:
                    k = 1;
                    break;
            }

            //switch (sideType)
            //{
            //    case ST.Top:
            //        ray[0] = 1 * k;
            //        ray[1] = 1;
            //        break;
            //    case ST.Right:
            //        ray[0] = 1;
            //        ray[1] = -1 * k;
            //        break;
            //    case ST.Down:
            //        ray[0] = -1 * k;
            //        ray[1] = -1;
            //        break;
            //    case ST.Left:
            //        ray[0] = -1;
            //        ray[1] = 1 * k;
            //        break;
            //}

            switch (sideType)
            {
                case ST.Top:
                    ray[0] = 1 * k;
                    ray[1] = -1;
                    break;
                case ST.Right:
                    ray[0] = -1;
                    ray[1] = -1 * k;
                    break;
                case ST.Down:
                    ray[0] = -1 * k;
                    ray[1] = 1;
                    break;
                case ST.Left:
                    ray[0] = 1;
                    ray[1] = 1 * k;
                    break;
            }

            ray[0] *= inCast ? 1 : -1;
            ray[1] *= inCast ? 1 : -1;
        }
    }
}
