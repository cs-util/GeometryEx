using System;
using System.Collections.Generic;
using Xunit;
using Elements;
using Elements.Geometry;
using GeometryEx;

namespace GeometryExTests
{
    public class TriangleExTests
    {
        [Fact]
        public void Area()
        {
            var t =
                new Triangle(new Vertex(Vector3.Origin),
                             new Vertex(Vector3.XAxis),
                             new Vertex(Vector3.YAxis));
            Assert.True(t.Area().NearEqual(0.5));
        }

        [Fact]
        public void Edges()
        {
            var t1 = 
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0)));
            var t2 =
                new Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)));
            var edges1 = t1.Edges();
            var edges2 = t2.Edges();
            Assert.Equal(3, edges1.Count);
            Assert.Equal(3, edges2.Count);
            Assert.True(new Line(new Vector3(2.0, 2.0, 12.0), new Vector3(9.0, 2.0, 12.0)).IsListed(edges1));
            Assert.True(new Line(new Vector3(2.0, 2.0, 12.0), new Vector3(5.0, 4.0, 10.0)).IsListed(edges1));
            Assert.True(new Line(new Vector3(9.0, 2.0, 12.0), new Vector3(5.0, 4.0, 10.0)).IsListed(edges2));
            Assert.True(new Line(new Vector3(9.0, 2.0, 12.0), new Vector3(5.0, 11.0, 10.0)).IsListed(edges2));
        }
    }
}
