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
            var perimeter =
                new Polygon
                 (
                     new[]
                     {
                            new Vector3(22.4239, -11.4625,0.0000),
                            new Vector3(41.3779, -11.4625,0.0000),
                            new Vector3(41.3779, 35.0399, 0.0000),
                            new Vector3(-22.4239, 35.0399, 0.0000)
                     }
                 );
            var grid = new Grid(perimeter, 31.900910660932219, 11.625584573198932, 69.39415549558927, 1.0, true, true, GridPosition.CenterXY);
            var model = new Model();
            model.AddElement(new Panel(perimeter, BuiltInMaterials.Glass, null, null, Guid.NewGuid(), ""));
            foreach (var line in grid.Lines)
            {
                //model.AddElement(new Beam(line, new Profile(Polygon.Rectangle(2.0, 2.0)), BuiltInMaterials.Steel));
            }
            var matl = new Material("test", Palette.Aqua);
            foreach (var cell in grid.Cells)
            {
                model.AddElement(new Space(new Profile(cell), 3.0, matl));
            }
            model.ToGlTF("../../../../gridCreate.glb");
        }

        [Fact]
        public void Intersection()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0, 0.0),
                    new Vector3(8.0, 0.0, 0.0),
                    new Vector3(8.0, 8.0, 0.0),
                    new Vector3(0.0, 8.0, 0.0)
                }
            );
            var grid = new Grid(perimeter, 2.0, 2.0, 0.0, 0.0, false, false, GridPosition.CenterSpan);
            var intersect = grid.Intersection(1, 1);
            Assert.Equal(3.0, intersect.X);
            Assert.Equal(3.0, intersect.Y);

            intersect = grid.Intersection(2, 2);
            Assert.Equal(5.0, intersect.X);
            Assert.Equal(5.0, intersect.Y);

            grid = new Grid(perimeter, 2.0, 2.0, 0.0, 0.0, false, false, GridPosition.CenterXY);
            intersect = grid.Intersection(1, 1);
            Assert.Equal(4.0, intersect.X);
            Assert.Equal(4.0, intersect.Y);

            intersect = grid.Intersection(2, 2);
            Assert.Equal(6.0, intersect.X);
            Assert.Equal(6.0, intersect.Y);
        }

        [Fact]
        public void Intersections()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0, -1.0),
                    new Vector3(8.0, 0.0, -1.0),
                    new Vector3(8.0, 8.0, -1.0),
                    new Vector3(0.0, 8.0, -1.0)
                }
            );
            var grid = new Grid(perimeter, 2.0, 2.0, 45.0, 0.0, false, false, GridPosition.CenterSpan);
            Assert.Equal(36, grid.Intersections.Count);

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
            Assert.Equal(12, grid.Lines.Count);
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
            Assert.Equal(12, grid.Lines.Count);
            model.ToGlTF("../../../../gridIntersectsCulled.glb");
        }

        [Fact]
        public void Points()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0, 0.0),
                    new Vector3(8.0, 0.0, 0.0),
                    new Vector3(8.0, 8.0, 0.0),
                    new Vector3(0.0, 8.0, 0.0)
                }
            );
            var grid = new Grid(perimeter, 2.0, 2.0, 0.0, 0.0, false, false, GridPosition.CenterSpan);
            var points = grid.Points;
            Assert.Equal(32, points.Count);
        }

        [Fact]
        public void PointsAlong()
        {
            var perimeter = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(8.0, 0.0),
                    new Vector3(8.0, 8.0),
                    new Vector3(0.0, 8.0)
                }
            );
            var grid = new Grid(perimeter, 2.0, 2.0, 0.0, 0.0, false, false, GridPosition.CenterXY);
            var points = grid.PointsAlongX(0);
            Assert.Equal(5, points.Count);

            points = grid.PointsAlongY(0);
            Assert.Equal(5, points.Count);
        }
    }
}