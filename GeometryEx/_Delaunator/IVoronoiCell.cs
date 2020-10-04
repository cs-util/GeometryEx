using System.Collections.Generic;

namespace GeometryEx
{
    public interface IVoronoiCell
    {
        IPoint[] Points { get; }
        int Index { get; }
    }
}
