using System.Collections.Generic;

namespace GeometryEx
{
    public interface ITriangle
    {
        IEnumerable<IPoint> Points { get; }
        int Index { get; }
    }
}
