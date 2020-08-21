using System.Collections.Generic;
using Xunit;
using Elements;
using Elements.Geometry;
using GeometryEx;

namespace GeometryExTests
{
    public class MeshExTests
    {
        [Fact]
        public void Edges()
        {
            var triangles = new List<Triangle>
            {
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var edges = mesh.Edges();
            Assert.Equal(11, edges.Count);
        }

        [Fact]
        public void Planes()
        {
            var triangles = new List<Triangle>
            {
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var planes = mesh.Planes(MeshEx.Normal.X);
            Assert.Equal(3, planes.Count);
            planes = mesh.Planes(MeshEx.Normal.Y);
            Assert.Equal(4, planes.Count);
            planes = mesh.Planes(MeshEx.Normal.Z);
            Assert.Equal(2, planes.Count);
        }
    }
}
