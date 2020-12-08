using System.Linq;
using System.Collections.Generic;
using Xunit;
using Elements;
using Elements.Geometry;
using Elements.Geometry.Solids;
using GeometryEx;

namespace GeometryExTests
{
    public class MeshExTests
    {
        [Fact]
        public void Area()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(0.0, 0.0, 0.0)),
                                               new Vertex(new Vector3(2.0, 0.0, 0.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 0.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(0.0, 0.0, 0.0)),
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
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
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
            adjTriangles.Clear();
            adjTriangles = mesh.AdjacentTriangles(triangles.First());
            Assert.Equal(2, adjTriangles.Count);
        }

        [Fact]
        public void AdjacentPoints()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var pnt1 = new Vector3(2.0, 2.0, 12.0);
            var pnt2 = new Vector3(5.0, 4.0, 10.0);
            var pnt3 = new Vector3(9.0, 13.0, 12.0);
            var adjPoints = mesh.AdjacentPoints(pnt1);
            Assert.Equal(3, adjPoints.Count);
            adjPoints = mesh.AdjacentPoints(pnt2);
            Assert.Equal(4, adjPoints.Count);
            adjPoints = mesh.AdjacentPoints(pnt3);
            Assert.Equal(3, adjPoints.Count);
        }

        [Fact]
        public void AverageAt()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
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
        public void ConcaveEdges()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                //south triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(6.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(6.0, 4.0, 10.0))),

                //north triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(6.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),

                //west triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),

                //east triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0)))

            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var conEdges = mesh.ConcaveEdges(Vector3.ZAxis);
            Assert.Equal(8, conEdges.Count);
        }

        [Fact]
        public void ConcavePoints()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var cncPoints = mesh.ConcavePoints(Vector3.ZAxis);
            Assert.Equal(2, cncPoints.Count);
        }

        [Fact]
        public void ConvexPoints()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 14.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 14.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 14.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 14.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var cnvPoints = mesh.ConvexPoints(Vector3.ZAxis);
            Assert.Equal(2, cnvPoints.Count);
        }

        [Fact]
        public void ConvexEdges()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 14.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 14.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 14.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 14.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 14.0))),


                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(15.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 14.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 14.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 14.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var edges = mesh.Edges();
            var ridges = mesh.ConvexEdges(Vector3.ZAxis);
            Assert.Equal(2, ridges.Count);
        }

        [Fact]
        public void Edges()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
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
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
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
        public void EdgesInterior()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var edges = mesh.EdgesInterior();
            Assert.Equal(7, edges.Count);
        }

        [Fact]
        public void EdgesPerimeters()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                //south triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0))),

                //north triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),

                //west triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),

                //east triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
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
        public void HighestFrom()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),

                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(15.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),

                new Elements.Geometry.Triangle(new Vertex(new Vector3(0.0, 0.0, 14.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(0.0, 0.0, 14.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(0.0, 0.0, 14.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(0.0, 15.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(0.0, 15.0, 14.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),


            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var lowPnt1 = new Vector3(5.0, 4.0, 10.0);
            var lowPnt2 = new Vector3(12.0, 11.0, 10.0);
            var hiFrom1 = mesh.HighestFrom(lowPnt1, Vector3.ZAxis);
            var hiFrom2 = mesh.HighestFrom(lowPnt2, Vector3.ZAxis);
            Assert.Equal(2, hiFrom1.Count);
            Assert.Equal(3, hiFrom2.Count);
        }

        [Fact]
        public void IsConcaveLine()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),


                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(15.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var edges = mesh.Edges();
            var valleys = new List<Line>();
            foreach (var e in edges)
            {
                if (mesh.IsConcave(e, Vector3.ZAxis))
                {
                    valleys.Add(e);
                }
            }
            Assert.Equal(10, valleys.Count);
        }

        [Fact]
        public void IsConcavePoint()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var pnt1 = new Vector3(2.0, 2.0, 12.0);
            var pnt2 = new Vector3(5.0, 4.0, 10.0);
            var pnt3 = new Vector3(9.0, 13.0, 12.0);
            Assert.False(mesh.IsConcave(pnt1, Vector3.ZAxis));
            Assert.True(mesh.IsConcave(pnt2, Vector3.ZAxis));
            Assert.False(mesh.IsConcave(pnt3, Vector3.ZAxis));
        }

        [Fact]
        public void IsConvexLine()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 14.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 14.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 14.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var line1 = new Line(new Vector3(2.0, 2.0, 12.0), new Vector3(9.0, 2.0, 12.0));
            var line2 = new Line(new Vector3(5.0, 4.0, 14.0), new Vector3(5.0, 11.0, 14.0));
            var line3 = new Line(new Vector3(9.0, 13.0, 12.0), new Vector3(2.0, 13.0, 12.0));
            Assert.False(mesh.IsConvex(line1, Vector3.ZAxis));
            Assert.True(mesh.IsConvex(line2, Vector3.ZAxis));
            Assert.False(mesh.IsConvex(line3, Vector3.ZAxis));
        }

        [Fact]
        public void IsConvexPoint()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 14.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 14.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 14.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var pnt1 = new Vector3(2.0, 2.0, 12.0);
            var pnt2 = new Vector3(5.0, 4.0, 14.0);
            var pnt3 = new Vector3(9.0, 13.0, 12.0);
            Assert.False(mesh.IsConvex(pnt1, Vector3.ZAxis));
            Assert.True(mesh.IsConvex(pnt2, Vector3.ZAxis));
            Assert.False(mesh.IsConvex(pnt3, Vector3.ZAxis));
        }

        [Fact]
        public void IsFlat()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            Assert.False(mesh.IsFlat());

            triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            Assert.True(mesh.IsFlat());
        }

        [Fact]
        public void LowestFrom()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),

                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(15.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(15.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(12.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(12.0, 5.0, 10.0))),

                new Elements.Geometry.Triangle(new Vertex(new Vector3(0.0, 0.0, 14.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(0.0, 0.0, 14.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(0.0, 0.0, 14.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(0.0, 15.0, 14.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(0.0, 15.0, 14.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),


            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var hiPnt1 = new Vector3(0.0, 15.0, 14.0);
            var hiPnt2 = new Vector3(15.0, 13.0, 12.0);
            var loFrom1 = mesh.LowestFrom(hiPnt1, Vector3.ZAxis);
            var loFrom2 = mesh.LowestFrom(hiPnt2, Vector3.ZAxis);
            Assert.Equal(4, loFrom1.Count);
            Assert.Single(loFrom2);
        }

        [Fact]
        public void MeshClosed()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                //topside
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                //underside
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 8.0)),
                             new Vertex(new Vector3(9.0, 2.0, 8.0)),
                             new Vertex(new Vector3(2.0, 13.0, 8.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 8.0)),
                             new Vertex(new Vector3(9.0, 2.0, 8.0)),
                             new Vertex(new Vector3(9.0, 13.0, 8.0))),
                //west
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 8.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 8.0)),
                             new Vertex(new Vector3(2.0, 2.0, 8.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                ////east
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 2.0, 8.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 8.0)),
                             new Vertex(new Vector3(9.0, 2.0, 8.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                //north
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 8.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 8.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 8.0))),
                //south
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 8.0)),
                             new Vertex(new Vector3(9.0, 2.0, 8.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 2.0, 8.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var edges = mesh.EdgesPerimeters();
            var bndPoints = mesh.PointsBoundary();
            Assert.Empty(edges);
            Assert.Empty(bndPoints);
        }

        [Fact]
        public void Planes()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
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
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
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
            var triangles = new List<Elements.Geometry.Triangle>
            {
                //south triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0))),

                //north triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),

                //west triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),

                //east triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
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
            var triangles = new List<Elements.Geometry.Triangle>
            {
                //south triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0))),

                //north triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),

                //west triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),

                //east triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
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
        public void ThirdPoints()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(9.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(9.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(9.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                                               new Vertex(new Vector3(2.0, 2.0, 12.0)),
                                               new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(5.0, 4.0, 10.0)),
                                               new Vertex(new Vector3(5.0, 11.0, 10.0)),
                                               new Vertex(new Vector3(2.0, 13.0, 12.0)))
            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var edge = new Line(new Vector3(5.0, 11.0, 10.0), new Vector3(5.0, 4.0, 10.0));
            var thirdPoints = mesh.ThirdPoints(edge);
            Assert.Equal(2, thirdPoints.Count);
            edge = new Line(new Vector3(2.0, 2.0, 12.0), new Vector3(2.0, 13.0, 12.0));
            thirdPoints = mesh.ThirdPoints(edge);
            Assert.Single(thirdPoints);
        }

        [Fact]
        public void ToIndexedVertices()
        {
            var triangles = new List<Elements.Geometry.Triangle>
            {
                //south triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(6.0, 4.0, 10.0))),

                //north triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0))),

                //west triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0)),
                             new Vertex(new Vector3(2.0, 13.0, 12.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(2.0, 2.0, 12.0)),
                             new Vertex(new Vector3(5.0, 4.0, 10.0)),
                             new Vertex(new Vector3(5.0, 11.0, 10.0))),

                //east triangles
                new Elements.Geometry.Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)),
                             new Vertex(new Vector3(6.0, 11.0, 10.0))),
                new Elements.Geometry.Triangle(new Vertex(new Vector3(6.0, 4.0, 10.0)),
                             new Vertex(new Vector3(9.0, 2.0, 12.0)),
                             new Vertex(new Vector3(9.0, 13.0, 12.0)))

            };
            var mesh = new Mesh();
            triangles.ForEach(t => mesh.AddTriangle(t));
            var idxVerts = mesh.ToIndexedVertices();
            Assert.Equal(8, idxVerts.triangles.Count);
            Assert.Equal(8, idxVerts.vertices.Count);
        }
    }
}
