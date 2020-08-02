using System.Collections.Generic;
using Xunit;
using Elements;
using Elements.Geometry;
using GeometryEx;

namespace GeometryExTests
{
    public class ArcExTests
    {
        [Fact]
        public void Divide()
        {
            var arc = new Arc(Vector3.Origin, 100.0, 0.0, 180.0);            
            Assert.Equal(25.0, arc.Divide(24).Count);
        }

        [Fact]
        public void ToLines()
        {
            var arc = new Arc(Vector3.Origin, 100.0, 0.0, 180.0);
            Assert.Equal(24.0, arc.ToLines(24).Count);
        }

        [Fact]
        public void ToPolyline()
        {
            var arc = new Arc(Vector3.Origin, 100.0, 0.0, 180.0);
            var pline = arc.ToPolyline(24);
            Assert.Equal(25.0, pline.Vertices.Count);
        }
    }
}
