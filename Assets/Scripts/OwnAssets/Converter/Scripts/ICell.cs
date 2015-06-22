using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProdToFerra2D
{
    public interface ICell
    {
        int X { get; }
        int Y { get; }
        CellType Type { get; }
    }
}
