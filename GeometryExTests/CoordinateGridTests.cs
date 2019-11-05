using System.Collections.Generic;
using Xunit;
using Elements;
using Elements.Geometry;
using Elements.Serialization.glTF;
using GeometryEx;

namespace GeometryExTests
{
    public class CoordinateGridTests
    {
        [Fact]
        public void CoordinateGrid()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0, 0),
                    new Vector3(20, 0),
                    new Vector3(20, 30),
                    new Vector3(0, 30)
                }
            );
            var grid = new CoordinateGrid(perimeter);
            var model = new Model();
            foreach (var point in grid.Available)
            {
                model.AddElement(new Column(point, 4.0, new Profile(Polygon.Rectangle(0.1, 0.1))));
            }
            model.ToGlTF("../../../../CoordinateGrid.glb");
            Assert.Equal(600, grid.Available.Count);
        }

        [Fact]
        public void Allocate()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0, 0),
                    new Vector3(60, 0),
                    new Vector3(60, 36),
                    new Vector3(0, 36)
                }
            );
            var grid = new CoordinateGrid(perimeter);
            Assert.Equal(2160, grid.Available.Count);
            var allocate1 = new Polygon
            (
                new[]
                {
                    new Vector3(10, 10),
                    new Vector3(20, 10),
                    new Vector3(20, 20),
                    new Vector3(10, 20)
                }
            );
            grid.Allocate(allocate1);
            Assert.Equal(2060, grid.Available.Count);
            var allocate2 = new Polygon
            (
                new[]
                {
                    new Vector3(30, 10),
                    new Vector3(40, 10),
                    new Vector3(40, 30),
                    new Vector3(30, 30)
                }
            );
            grid = new CoordinateGrid(perimeter);
            var allocate = new List<Polygon> { allocate1, allocate2 };
            grid.Allocate(allocate);
            Assert.Equal(1860, grid.Available.Count);
        }

        [Fact]
        public void AllocatedNearTo()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0, 0),
                    new Vector3(60, 0),
                    new Vector3(60, 36),
                    new Vector3(0, 36)
                }
            );
            var allocated = new List<Polygon>
            {
                new Polygon
                (
                    new []
                    {
                        Vector3.Origin,
                        new Vector3(8, 0),
                        new Vector3(8, 9),
                        new Vector3(0, 9)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(52, 0),
                        new Vector3(60, 0),
                        new Vector3(60, 6),
                        new Vector3(52, 6)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(24, 33),
                        new Vector3(32, 33),
                        new Vector3(32, 36),
                        new Vector3(24, 36)
                    }
                )
            };
            var grid = new CoordinateGrid(perimeter);
            foreach(Polygon polygon in allocated)
            {
                grid.Allocate(polygon);
            }
            var nearPoint = grid.AllocatedNearTo(new Vector3(26.6, 34.1));
            Assert.Equal(26.5, nearPoint.X);
            Assert.Equal(35.5, nearPoint.Y);
        }

        [Fact]
        public void AllocatedRandom()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0, 0),
                    new Vector3(60, 0),
                    new Vector3(60, 36),
                    new Vector3(0, 36)
                }
            );
            var grid = new CoordinateGrid(perimeter);
            var allocate = new Polygon
            (
                new[]
                {
                    new Vector3(10, 10),
                    new Vector3(20, 10),
                    new Vector3(20, 20),
                    new Vector3(10, 20)
                }
            );
            grid.Allocate(allocate);
            var point = grid.AllocatedRandom();
            Assert.Contains(point, grid.Allocated);
        }

        [Fact]
        public void AvailableMax()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(5, 5),
                    new Vector3(60, 5),
                    new Vector3(60, 36),
                    new Vector3(5, 36)
                }
            );
            var grid = new CoordinateGrid(perimeter);
            var max = grid.AvailableMax();
            Assert.Equal(60, max.X);
            Assert.Equal(36, max.Y);
        }

        [Fact]
        public void AvailableMin()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(5, 5),
                    new Vector3(60, 5),
                    new Vector3(60, 36),
                    new Vector3(5, 36)
                }
            );
            var grid = new CoordinateGrid(perimeter);
            var min = grid.AvailableMin();
            Assert.Equal(5, min.X);
            Assert.Equal(5, min.Y);
        }

        [Fact]
        public void AvailableNearTo()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0, 0),
                    new Vector3(60, 0),
                    new Vector3(60, 36),
                    new Vector3(0, 36)
                }
            );
            var grid = new CoordinateGrid(perimeter);
            var nearPoint = grid.AvailableNearTo(new Vector3(50.6, 40.1));
            Assert.Equal(29.5, nearPoint.X);
            Assert.Equal(17.5, nearPoint.Y);
        }

        [Fact]
        public void AvailableRandom()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0, 0),
                    new Vector3(60, 0),
                    new Vector3(60, 36),
                    new Vector3(0, 36)
                }
            );
            var grid = new CoordinateGrid(perimeter);
            var allocate = new Polygon
            (
                new[]
                {
                    new Vector3(10, 10),
                    new Vector3(20, 10),
                    new Vector3(20, 20),
                    new Vector3(10, 20)
                }
            );
            grid.Allocate(allocate);
            var point = grid.AvailableRandom();
            Assert.Contains(point, grid.Available);
        }
    }
}
