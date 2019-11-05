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
            var arc = new Arc(Vector3.Origin, 100.0, 150.0, 180.0);            
            Assert.Equal(25.0, arc.Divide(24).Count);
        }
    }
}
