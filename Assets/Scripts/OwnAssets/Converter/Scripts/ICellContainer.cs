using ProdToFerra2D.Internal;
using System.Collections.Generic;

namespace ProdToFerra2D
{
    public interface ICellContainer
    {
        IEnumerable<ICell> GetCells();

        List<ICell> AllCells { get; }

        ContainerType Type { get; }
    }
}
