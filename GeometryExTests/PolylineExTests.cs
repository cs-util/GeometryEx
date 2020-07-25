using System;
using System.Collections.Generic;
using Xunit;
using Elements.Geometry;
using GeometryEx;


namespace GeometryExTests
{
    public class PolylineExTests
    {
        [Fact]
        public void Simplify()
        {
            var points = new List<Vector3>()
            {
                Vector3.Origin,
                new Vector3(5.0, 0.0),
                new Vector3(7.0, 7.0),
                new Vector3(10.0, 15.0),
                new Vector3(15.0, 20.0),
                new Vector3(10.0, 20.0),
                new Vector3(5.0, 10.0),
            };
            var polyline = new Polyline(points).Simplify(0.5);
            Assert.Equal(3, polyline.Vertices.Count);
        }
    }
}
