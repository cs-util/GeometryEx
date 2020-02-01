using System.Collections.Generic;
using Xunit;
using Elements;
using Elements.Geometry;
using GeometryEx;

namespace GeometryExTests
{
    public class PolygonExTests
    {
        [Fact]
        public void AspectRatio()
        {
            var polygon = 
                new Polygon
                (
                    new[]
                    {
                        new Vector3(0.0, 0.0),
                        new Vector3(10.0, 0.0),
                        new Vector3(10.0, 20.0),
                        new Vector3(0.0, 20.0)
                    }
                );
            Assert.Equal(2.0, polygon.AspectRatio());
        }

        [Fact]
        public void Contains()
        {
            var polygon = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(10.0, 0.0),
                    new Vector3(10.0, 10.0),
                    new Vector3(0.0, 10.0)
                }
            );
            var pnt1 = Vector3.Origin;
            var pnt2 = new Vector3(5.0, 5.0);
            var pnt3 = new Vector3(20.0, 20.0);

            Assert.False(polygon.Contains(pnt1));
            Assert.True(polygon.Contains(pnt2));
            Assert.False(polygon.Contains(pnt3));
        }

        [Fact]
        public void Covers()
        {
            var p1 = new Polygon
            (
                new[]
                {
                    new Vector3(0, 0),
                    new Vector3(20, 0),
                    new Vector3(20, 20),
                    new Vector3(0, 20)
                }
            );
            var p2 = new Polygon
            (
                new[]
                {
                    new Vector3(0, 0),
                    new Vector3(10, 0),
                    new Vector3(10, 10),
                    new Vector3(0, 10)
                }
            );
            Assert.True(p1.Covers(p2));
            Assert.True(p1.Covers(p2.Reversed()));
            Assert.False(p1.Covers(new Vector3(-1.1, -1.1)));
            Assert.True(p1.Covers(new Vector3(2.0, 5.0)));
            Assert.True(p1.Covers(new Vector3(2.0, 0.0)));
        }

        [Fact]
        public void Difference()
        {
            var polygon = 
                new Polygon
                (
                    new[]
                    {
                        new Vector3(0.0, 3.0),
                        new Vector3(11.0, 3.0),
                        new Vector3(11.0, 12.0),
                        new Vector3(0.0, 12.0),
                        new Vector3(0.0, 9.0),
                        new Vector3(8.0, 9.0),
                        new Vector3(8.0, 6.0),
                        new Vector3(0.0, 6.0)
                    }
                );
            var difference = 
                new Polygon
                (
                    new[]
                    {
                        new Vector3(2.0, 0.0),
                        new Vector3(5.0, 0.0),
                        new Vector3(5.0, 15.0),
                        new Vector3(2.0, 15.0)
                    }
                );
            var diffList = polygon.Difference(difference);
            Assert.Equal(3, diffList.Count);

        }

        [Fact]
        public void Disjoint()
        {
            var p1 = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(4.0, 0.0),
                    new Vector3(4.0, 4.0),
                    new Vector3(0.0, 4.0)
                }
            );
            var p2 = new Polygon
            (
                new[]
                {
                    new Vector3(5.0, 0.0),
                    new Vector3(10.0, 0.0),
                    new Vector3(10.0, 10.0),
                    new Vector3(5.0, 10.0)
                }
            );
            var p3 = new Polygon
            (
                new[]
                {
                    new Vector3(3.0, 2.0),
                    new Vector3(7.0, 2.0),
                    new Vector3(7.0, 6.0),
                    new Vector3(3.0, 6.0)
                }
            );
            Assert.True(p1.Disjoint(p2));
            Assert.False(p1.Disjoint(p3));
            Assert.False(p2.Disjoint(p3));
        }

        [Fact]
        public void FindInternalPoints()
        {
            var within = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(100.0, 0.0),
                    new Vector3(100.0, 25.0),
                    new Vector3(25.0, 25.0),
                    new Vector3(25.0, 100.0),
                    new Vector3(0.0, 100.0),
                }
            );
            var points = within.FindInternalPoints();
            Assert.Equal(172, points.Count);
            points = within.FindInternalPoints(0.5);
            Assert.Equal(344, points.Count);
        }

        [Fact]
        public void Fits()
        {
            var within = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(40.0, 0.0),
                    new Vector3(40.0, 40.0),
                    new Vector3(0.0, 40.0)
                }
            );
            var room = new Polygon
            (
                new[]
                {
                    new Vector3(40.0, 40.0),
                    new Vector3(34.51511960021, 40.0),
                    new Vector3(34.51511960021, 35.0773767097941),
                    new Vector3(40.0, 35.0773767097941)
                }
            );
            Assert.True(room.Fits(within, null));
        }

        [Fact]
        public void Intersects()
        {
            var p1 = new Polygon
            (
                new[]
                {
                    new Vector3(10.0, 0.0),
                    new Vector3(20.0, 0.0),
                    new Vector3(20.0, 20.0),
                    new Vector3(10.0, 20.0)
                }
            );

            var p2 = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(20.0, 0.0),
                    new Vector3(20.0, 10.0),
                    new Vector3(0.0, 10.0)
                }
            );

            var p3 = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(5.0, 0.0),
                    new Vector3(5.0, 5.0),
                    new Vector3(0.0, 5.0)
                }
            );

            var p4 = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(5.0, 0.0),
                    new Vector3(5.0, 5.0),
                    new Vector3(0.0, 5.0)
                }
            );

            var polygons = new List<Polygon> { p2, p3 };

            Assert.True(p1.Intersects(p2));
            Assert.True(p2.Intersects(p3));
            Assert.False(p1.Intersects(p3));
            Assert.True(p3.Intersects(p4));
            Assert.True(p1.Intersects(polygons));
        }

        [Fact]
        public void IsClockWise()
        {
            var p1 = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 5.0),
                    new Vector3(5.0, 5.0),
                    new Vector3(0.0, 10.0),
                    new Vector3(10.0, 10.0),
                    new Vector3(10.0, 5.0),
                    new Vector3(5.0, 0.0),
                    new Vector3(0.0, 0.0)
                 }
            );
            Assert.True(p1.IsClockWise());
            var verts = new List<Vector3>(p1.Vertices);
            verts.Reverse();
            var p2 = new Polygon(verts.ToArray());
            Assert.False(p2.IsClockWise());
        }

        [Fact]
        public void MoveFromTo()
        {
            var polygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(0.0, 0.0),
                        new Vector3(4.0, 0.0),
                        new Vector3(4.0, 4.0),
                        new Vector3(0.0, 4.0)
                    }
                );
            polygon = polygon.MoveFromTo(Vector3.Origin, new Vector3(4.0, 4.0, 4.0));
            Assert.Contains(new Vector3(4.0, 4.0, 4.0), polygon.Vertices);
            Assert.Contains(new Vector3(8.0, 8.0, 4.0), polygon.Vertices);
        }

        [Fact]
        public void RewindFrom()
        {
            var polygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(0.0, 0.0),
                        new Vector3(4.0, 0.0),
                        new Vector3(4.0, 4.0),
                        new Vector3(0.0, 4.0)
                    }
                );
            polygon = polygon.RewindFrom(new Vector3(4.0, 0.0));
            Assert.Equal(4.0, polygon.Vertices[0].X);
            Assert.Equal(0.0, polygon.Vertices[0].Y);

            polygon = polygon.RewindFrom(new Vector3(0.0, 4.0));
            Assert.Equal(0.0, polygon.Vertices[0].X);
            Assert.Equal(4.0, polygon.Vertices[0].Y);

            polygon = polygon.RewindFrom(new Vector3(1.0, 4.0));
            Assert.Null(polygon);

            polygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(0.0, 0.0),
                        new Vector3(4.0, 0.0),
                        new Vector3(4.0, 4.0),
                        new Vector3(0.0, 4.0)
                    }
                );

            polygon = polygon.RewindFrom(1);
            Assert.Equal(4.0, polygon.Vertices[0].X);
            Assert.Equal(0.0, polygon.Vertices[0].Y);

            polygon = polygon.RewindFrom(5);
            Assert.Null(polygon);
        }

        [Fact]
        public void Rotate()
        {
            var polygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(0.0, 0.0),
                        new Vector3(4.0, 0.0),
                        new Vector3(4.0, 4.0),
                        new Vector3(0.0, 4.0)
                    }
                );
            polygon = polygon.Rotate(Vector3.Origin, 90);
            Assert.Contains(new Vector3(-4.0, 4.0), polygon.Vertices);
            Assert.Contains(new Vector3(-4.0, 0.0), polygon.Vertices);
        }

        [Fact]
        public void Touches()
        {
            var p1 = new Polygon
            (
                new[]
                {
                    new Vector3(10.0, 0.0),
                    new Vector3(20.0, 0.0),
                    new Vector3(20.0, 20.0),
                    new Vector3(10.0, 20.0)
                }
            );

            var p2 = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(20.0, 0.0),
                    new Vector3(20.0, 10.0),
                    new Vector3(0.0, 10.0)
                }
            );

            var p3 = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(10.0, 0.0),
                    new Vector3(5.0, 5.0),
                    new Vector3(0.0, 5.0)
                }
            );

            var point1 = new Vector3(-1.1, -1.1);
            var point2 = new Vector3(0.0, 5.0);

            Assert.False(p1.Touches(p2));
            Assert.True(p1.Touches(p3));
            Assert.False(p2.Touches(p3));

            Assert.False(p1.Touches(point1));
            Assert.True(p2.Touches(point2));
        }
    }
}
