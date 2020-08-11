using System.Collections.Generic;
using Xunit;
using Elements;
using Elements.Geometry;
using GeometryEx;

namespace GeometryExTests
{
    public class ConvexHullTests
    {
        [Fact]
        public void MakeHull()
        {
            var points =
                new List<Vector3>
                {
                    new Vector3(6.0, 11.0),
                    new Vector3(6.0, 7.0),
                    new Vector3(2.0, 7.0),
                    new Vector3(2.0, 4.0),
                    new Vector3(6.0, 0.0),
                    new Vector3(9.0, 0.0),
                    new Vector3(9.0, 4.0),
                    new Vector3(13.0, 4.0),
                    new Vector3(13.0, 7.0),
                    new Vector3(9.0, 7.0),
                    new Vector3(9.0, 11.0),
                    new Vector3(6.0, 4.0)
                };
            points = ConvexHull.MakeHull(points);
            Assert.Equal(8.0, points.Count);
        }
    }
}
