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
        public void Create()
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
        public void Intersect()
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
            var intersect = grid.Intersection(1, 1);
            Assert.Equal(15.0, intersect.X);
            Assert.Equal(10.0, intersect.Y);

            intersect = grid.Intersection(2, 2);
            Assert.Equal(25.0, intersect.X);
            Assert.Equal(20.0, intersect.Y);

            grid = new Grid(perimeter, 10.0, 10.0, 0.0, GridPosition.CenterXY);
            intersect = grid.Intersection(1, 1);
            Assert.Equal(10.0, intersect.X);
            Assert.Equal(15.0, intersect.Y);

            intersect = grid.Intersection(2, 2);
            Assert.Equal(20.0, intersect.X);
            Assert.Equal(25.0, intersect.Y);
        }

        [Fact]
        public void Intersects()
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
            Assert.Equal(100, grid.Intersections.Count);

            var model = new Model();
            model.AddElement(new Panel(perimeter, BuiltInMaterials.Concrete, null, null, Guid.NewGuid(), ""));
            foreach (var line in grid.Lines)
            {
                model.AddElement(new Beam(line, new Profile(Polygon.Rectangle(0.1, 0.1)), BuiltInMaterials.Steel));
            }
            foreach (var point in grid.Intersections)
            {
                model.AddElement(new Column(point, 3.0, Polygon.Ngon(8), BuiltInMaterials.Steel));
            }
            Assert.Equal(20, grid.Lines.Count);
            model.ToGlTF("../../../../gridIntersects.glb");

            model = new Model();
            model.AddElement(new Panel(perimeter, BuiltInMaterials.Concrete, null, null, Guid.NewGuid(), ""));
            foreach (var line in grid.Lines)
            {
                model.AddElement(new Beam(line, new Profile(Polygon.Rectangle(0.1, 0.1)), BuiltInMaterials.Steel));
            }
            foreach (var point in grid.Intersections)
            {
                if (perimeter.Covers(Polygon.Ngon(8).MoveFromTo(Vector3.Origin, point)))
                {
                    model.AddElement(new Column(point, 3.0, Polygon.Ngon(8), BuiltInMaterials.Steel));
                }               
            }
            Assert.Equal(20, grid.Lines.Count);
            model.ToGlTF("../../../../gridIntersectsCulled.glb");
        }
    }
}