using System.Linq;
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
        public void Area()
        {
            var triangles = new List<Triangle>
            {
                new Triangle(new Vertex(new Vector3(0.0, 0.0, 0.0)),
                             new Vertex(new Vector3(2.0, 0.0, 0.0)),
                             new Vertex(new Vector3(2.0, 2.0, 0.0))),
                new Triangle(new Vertex(new Vector3(0.0, 0.0, 0.0)),
                             new Vertex(new Vector3(2.0, 2.0, 0.0)),
                             new Vertex(new Vector3(0.0, 2.0, 0.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            Assert.True(mesh.Area().NearEqual(4.0));
        }

        [Fact]
        public void AdjacentTriangles()
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
            var edge = new Line(new Vector3(5.0, 11.0, 10.0), new Vector3(5.0, 4.0, 10.0));
            var adjTriangles = mesh.AdjacentTriangles(edge);
            Assert.Equal(2, adjTriangles.Count);
            edge = new Line(new Vector3(2.0, 2.0, 12.0), new Vector3(2.0, 13.0, 12.0));
            adjTriangles = mesh.AdjacentTriangles(edge);
            Assert.Single(adjTriangles);
        }

        [Fact]
        public void AverageAt()
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
            var average = mesh.AverageAt(new Vector3(2.0, 2.0, 12.0));
            Assert.True(average.Z > 0.0);
            average = mesh.AverageAt(new Vector3(9.0, 13.0, 12.0));
            Assert.True(average.Z > 0.0);
            average = mesh.AverageAt(new Vector3(5.0, 4.0, 10.0));
            Assert.True(average.Z < 0.0);
            average = mesh.AverageAt(new Vector3(5.0, 11.0, 10.0));
            Assert.True(average.Z < 0.0);
        }

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
        public void EdgesAt()
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
            var point = new Vector3(2.0, 2.0, 12.0);
            var edges = mesh.EdgesAt(point);
            Assert.Equal(3, edges.Count);
            Assert.True(edges[0].End.IsAlmostEqualTo(point));
            Assert.True(edges[1].End.IsAlmostEqualTo(point));
            Assert.True(edges[2].End.IsAlmostEqualTo(point));
        }

        [Fact]
        public void IsConcavity()
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
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),

                new Triangle(new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(15.0, 2.0, 12.0)),
                             new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Triangle(new Vertex(new Vector3(15.0, 2.0, 12.0)),
                             new Vertex(new Vector3(15.0, 13.0, 12.0)),
                             new Vertex(new Vector3(12.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(15.0, 13.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(12.0, 11.0, 12.0))),
                new Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(12.0, 11.0, 10.0)),
                             new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Triangle(new Vertex(new Vector3(15.0, 2.0, 12.0)),
                             new Vertex(new Vector3(12.0, 11.0, 10.0)),
                             new Vertex(new Vector3(12.0, 5.0, 10.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var edges = mesh.Edges();
            var valleys = new List<Line>();
            foreach (var e in edges)
            {
                if (mesh.IsConcavity(e, Vector3.ZAxis))
                {
                    valleys.Add(e);
                }
            }
            Assert.Equal(2.0, valleys.Count);
        }

        [Fact]
        public void IsFlat()
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
            Assert.False(mesh.IsFlat());

            triangles = new List<Triangle>
            {
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 12.0))),
                new Triangle(new Vertex(new Vector3(5.0, 4.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 12.0))),
                new Triangle(new Vertex(new Vector3(5.0, 11.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 12.0))),
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 12.0))),
                new Triangle(new Vertex(new Vector3(5.0, 4.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            Assert.True(mesh.IsFlat());
        }

        [Fact]
        public void EdgesPerimeters()
        {
            var triangles = new List<Triangle>
            {
                //south triangles
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0))),

                //north triangles
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),

                //west triangles
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),

                //east triangles
                new Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)))

            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var edges = mesh.EdgesPerimeters();
            Assert.Equal(4, edges.First().Count);
            Assert.Equal(4, edges.Last().Count);
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
            Assert.Equal(2.0, planes.First().Origin.X);
            Assert.Equal(9.0, planes.Last().Origin.X);
            planes = mesh.Planes(MeshEx.Normal.Y);
            Assert.Equal(4, planes.Count);
            Assert.Equal(2.0, planes.First().Origin.Y);
            Assert.Equal(13.0, planes.Last().Origin.Y);
            planes = mesh.Planes(MeshEx.Normal.Z);
            Assert.Equal(2, planes.Count);
            Assert.Equal(10.0, planes.First().Origin.Z);
            Assert.Equal(12.0, planes.Last().Origin.Z);
        }

        [Fact]
        public void Points()
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
            var points = mesh.Points();
            Assert.Equal(6, points.Count);
        }

        [Fact]
        public void PointsBoundary()
        {
            var triangles = new List<Triangle>
            {
                //south triangles
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0))),

                //north triangles
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),

                //west triangles
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),

                //east triangles
                new Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)))

            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var bndPoints = mesh.PointsBoundary();
            Assert.Equal(4, bndPoints.Count);
            Assert.Contains(new Vector3(2.0, 2.0, 12.0), bndPoints);
            Assert.Contains(new Vector3(9.0, 2.0, 12.0), bndPoints);
            Assert.Contains(new Vector3(2.0, 13.0, 12.0), bndPoints);
            Assert.Contains(new Vector3(9.0, 13.0, 12.0), bndPoints);
        }

        [Fact]
        public void PointsInterior()
        {
            var triangles = new List<Triangle>
            {
                //south triangles
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0))),

                //north triangles
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),

                //west triangles
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),

                //east triangles
                new Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)))

            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var inPoints = mesh.PointsInterior();
            Assert.Equal(4, inPoints.Count);
            Assert.Contains(new Vector3(5.0, 4.0, 10.0), inPoints);
            Assert.Contains(new Vector3(6.0, 4.0, 10.0), inPoints);
            Assert.Contains(new Vector3(5.0, 11.0, 10.0), inPoints);
            Assert.Contains(new Vector3(6.0, 11.0, 10.0), inPoints);
        }

        [Fact]
        public void ToIndexedVertices()
        {
            var triangles = new List<Triangle>
            {
                //south triangles
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0))),

                //north triangles
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),

                //west triangles
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),

                //east triangles
                new Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)))

            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var idxVerts = mesh.ToIndexedVertices();
            Assert.Equal(24, idxVerts.indices.Count);
            Assert.Equal(8, idxVerts.vertices.Count);
        }
    }
}
