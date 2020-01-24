using System.Collections.Generic;
using Xunit;
using System;
using Elements;
using Elements.Geometry;
using Elements.Serialization.glTF;
using GeometryEx;

namespace GeometryExTests
{
    public class GridTests
    {
        [Fact]
        public void GridCreate()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0, -1.0),
                    new Vector3(100.0, 0.0, -1.0),
                    new Vector3(100.0, 50.0, -1.0),
                    new Vector3(0.0, 50.0, -1.0)
                }
            );
            var grid = new Grid(perimeter, 10.0, 10.0, 45.0, GridPosition.CenterSpan);
            var model = new Model();
            model.AddElement(new Panel(perimeter, BuiltInMaterials.Concrete, null, null, Guid.NewGuid(), ""));
            foreach (var line in grid.Lines)
            {
                model.AddElement(new Beam(line, new Profile(Polygon.Rectangle(0.1, 0.1)), BuiltInMaterials.Steel));
            }
            Assert.Equal(20, grid.Lines.Count);
            model.ToGlTF("../../../../gridCenterSpan.glb");
        }

        [Fact]
        public void GridIntersect()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0, -1.0),
                    new Vector3(100.0, 0.0, -1.0),
                    new Vector3(100.0, 50.0, -1.0),
                    new Vector3(0.0, 50.0, -1.0)
                }
            );
            var grid = new Grid(perimeter, 10.0, 10.0, 0.0, GridPosition.CenterSpan);
            var intersect = grid.PointAt(1, 1);
            Assert.Equal(15.0, intersect.X);
            Assert.Equal(10.0, intersect.Y);

            intersect = grid.PointAt(2, 2);
            Assert.Equal(25.0, intersect.X);
            Assert.Equal(20.0, intersect.Y);

            grid = new Grid(perimeter, 10.0, 10.0, 0.0, GridPosition.CenterXY);
            intersect = grid.PointAt(1, 1);
            Assert.Equal(10.0, intersect.X);
            Assert.Equal(15.0, intersect.Y);

            intersect = grid.PointAt(2, 2);
            Assert.Equal(20.0, intersect.X);
            Assert.Equal(25.0, intersect.Y);
        }
    }
}