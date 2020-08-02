using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Elements;
using Elements.Serialization.glTF;
using Elements.Geometry;
using GeometryEx;

namespace GeometryExTests
{
    public class PolygonExTests
    {
        [Fact]
        public void AlignedAspectRatio()
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
            polygon = polygon.Rotate(Vector3.Origin, 45.0);
            Assert.True(2.0.NearEqual(polygon.AlignedAspectRatio()));
        }

        [Fact]
        public void AlignedBox()
        { 
            var polygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(5.0, 1.0),
                        new Vector3(5.0, 5.0),
                        new Vector3(2.0, 5.0)
                    }
                );
            var box = polygon.AlignedBox();
            Assert.Contains(new Vector3(3.92, 6.44, 0.0), box.Vertices);
            Assert.Contains(new Vector3(6.92, 2.44, 0.0), box.Vertices);
            Assert.Contains(new Vector3(2.0, 5.0, 0.0), box.Vertices);
            Assert.Contains(new Vector3(5.0, 1.0, 0.0), box.Vertices);
        }

        [Fact]
        public void AlignedBoxCorners()
        {
            var polygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(5.0, 1.0),
                        new Vector3(5.0, 5.0),
                        new Vector3(2.0, 5.0)
                    }
                );
            var boxCorners = polygon.AlignedBoxCorners();
            Assert.Contains(new Vector3(3.92, 6.44, 0.0), boxCorners);
            Assert.Contains(new Vector3(6.92, 2.44, 0.0), boxCorners);
            Assert.Contains(new Vector3(2.0, 5.0, 0.0), boxCorners);
            Assert.Contains(new Vector3(5.0, 1.0, 0.0), boxCorners);
        }

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
        public void CanMerge()
        {
            var polygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(0.0, 0.0),
                        new Vector3(5.0, 0.0),
                        new Vector3(5.0, 5.0),
                        new Vector3(0.0, 5.0)
                    }
                );
            var with = new List<Polygon>
            {
                new Polygon(
                    new []
                    {
                        new Vector3(3.0, 1.0),
                        new Vector3(7.0, 1.0),
                        new Vector3(7.0, 5.0),
                        new Vector3(3.0, 5.0)
                    }),
                new Polygon(
                    new[]
                    {
                        new Vector3(15.0, 30.0),
                        new Vector3(20.0, 30.0),
                        new Vector3(20.0, 36.0),
                        new Vector3(15.0, 36.0),
                    })
            };
            Assert.Equal(1.0, polygon.CanMerge(with).Count);
        }

        [Fact]
        public void ExpandToArea()
        {
            var polygon = new Polygon
            (
                new[]
                {
                    Vector3.Origin,
                    new Vector3(4.0, 0.0),
                    new Vector3(4.0, 4.0),
                    new Vector3(0.0, 4.0)
                }
            );
            var within = new Polygon
            (
                new[]
                {
                    new Vector3(1.0, 1.0),
                    new Vector3(8.0, 1.0),
                    new Vector3(8.0, 8.0),
                    new Vector3(1.0, 8.0)
                }
            );
            var among = new List<Polygon>
            {
                new Polygon(
                    new []
                    {
                        new Vector3(3.0, 1.0),
                        new Vector3(7.0, 1.0),
                        new Vector3(7.0, 5.0),
                        new Vector3(3.0, 5.0)
                    }),
                new Polygon(
                    new[]
                    {
                        new Vector3(1.0, 3.0),
                        new Vector3(2.0, 3.0),
                        new Vector3(2.0, 6.0),
                        new Vector3(1.0, 6.0),
                    })
            };
            polygon = polygon.ExpandToArea(30.0, 0.5, 0.1, Orient.C, within, among);
            var spaces = new List<Space>
            {
                new Space(polygon, 3.0, new Material("blue", Palette.Blue)),
                new Space(within, 0.1, new Material("aqua", Palette.Aqua)),
                new Space(among[0], 3.0, new Material("yellow", Palette.Aqua)),
                new Space(among[1], 3.0, new Material("green", Palette.Green))
            };
            var model = new Model();
            foreach (Space space in spaces)
            {
                model.AddElement(space);
            }
            model.ToGlTF("../../../../testOutput/expandToArea.glb");
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
        public void FitAmong()
        {
            var polygon = new Polygon
            (
                new[]
                {
                    Vector3.Origin,
                    new Vector3(4.0, 0.0),
                    new Vector3(4.0, 4.0),
                    new Vector3(0.0, 4.0)
                }
            );
            var among = new List<Polygon>
            {
                new Polygon(
                    new []
                    {
                        new Vector3(3.0, 1.0),
                        new Vector3(7.0, 1.0),
                        new Vector3(7.0, 5.0),
                        new Vector3(3.0, 5.0)
                    }),
                new Polygon(
                    new[]
                    {
                        new Vector3(1.0, 3.0),
                        new Vector3(2.0, 3.0),
                        new Vector3(2.0, 6.0),
                        new Vector3(1.0, 6.0),
                    })
            };
            polygon = polygon.FitAmong(among);
            Assert.Equal(10.0, polygon.Vertices.Count);
            var spaces = new List<Space>
            {
                new Space(polygon, 3.0, new Material("blue", Palette.Blue)),
                new Space(among[0], 3.0, new Material("yellow", Palette.Aqua)),
                new Space(among[1], 3.0, new Material("green", Palette.Green))
            };
            var model = new Model();
            foreach (Space space in spaces)
            {
                model.AddElement(space);
            }
            model.ToGlTF("../../../../testOutput/FitAmong.glb");
        }

        [Fact]
        public void FitMost()
        {
            var polygon = 
                new Polygon
                (
                    new[]
                    {
                        Vector3.Origin,
                        new Vector3(4.0, 0.0),
                        new Vector3(4.0, 4.0),
                        new Vector3(0.0, 4.0)
                    }
                );
            var within =
                new Polygon(
                    new[]
                    {
                        new Vector3(3.0, 3.0),
                        new Vector3(5.0, 3.0),
                        new Vector3(5.0, 6.0),
                        new Vector3(3.0, 6.0),
                    });
            polygon = polygon.FitMost(within);
            Assert.Equal(4.0, polygon.Vertices.Count);
            Assert.Equal(1.0, polygon.Area());
        }

        [Fact]
        public void FitTo()
        {
            var polygon = new Polygon
            (
                new[]
                {
                    Vector3.Origin,
                    new Vector3(4.0, 0.0),
                    new Vector3(4.0, 4.0),
                    new Vector3(0.0, 4.0)
                }
            );
            var within = new Polygon
            (
                new[]
                {
                    new Vector3(1.0, 1.0),
                    new Vector3(8.0, 1.0),
                    new Vector3(8.0, 8.0),
                    new Vector3(1.0, 8.0)
                }
            );
            var among = new List<Polygon>
            {
                new Polygon(
                    new []
                    {
                        new Vector3(3.0, 1.0),
                        new Vector3(7.0, 1.0),
                        new Vector3(7.0, 5.0),
                        new Vector3(3.0, 5.0)
                    }),
                new Polygon(
                    new[]
                    {
                        new Vector3(1.0, 3.0),
                        new Vector3(2.0, 3.0),
                        new Vector3(2.0, 6.0),
                        new Vector3(1.0, 6.0),
                    })
            };
            polygon = polygon.FitTo(within, among);
            var spaces = new List<Space>
            {
                new Space(polygon, 3.0, new Material("blue", new Color(0.0f, 0.0f, 1.0f, 0.6f))),
                new Space(within, 0.1, new Material("aqua", new Color(0.3f, 0.7f, 0.7f, 0.6f))),
                new Space(among[0], 3.0, new Material("yellow", new Color(1.0f, 0.9f, 0.1f, 0.6f))),
                new Space(among[1], 3.0, new Material("green", new Color(0.0f, 1.0f, 0.0f, 0.6f)))
            };
            var model = new Model();
            foreach (Space space in spaces)
            {
                model.AddElement(space);
            }
            model.ToGlTF("../../../../testOutput/FitTo.glb");
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
        public void Jigsaw()
        {
            var jigsaw = Shaper.L(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Jigsaw();
            Assert.Equal(6, jigsaw.Count);
            jigsaw = Shaper.C(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Jigsaw();
            Assert.Equal(8, jigsaw.Count);
            jigsaw = Shaper.X(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Jigsaw();
            Assert.Equal(12, jigsaw.Count);
            var model = new Model();
            var rooms = new List<Space>();
            foreach (var polygon in jigsaw)
            {
                model.AddElement(new Space(polygon, 4.0, new Material(Palette.Aqua, 0.0f, 0.0f, false, null, false, Guid.NewGuid(), "room")));
            }
            model.ToGlTF("../../../../testOutput/jigsaw.glb");
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
        public void Ribs()
        {
            var ribs = Shaper.L(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Ribs();
            Assert.Equal(6, ribs.Count);
            ribs = Shaper.C(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Ribs();
            Assert.Equal(8, ribs.Count);
            ribs = Shaper.X(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Ribs();
            Assert.Equal(12, ribs.Count);
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
        public void Simplify()
        {
            var points = new List<Vector3>()
            {
                Vector3.Origin,
                new Vector3(5.0, 0.0),
                new Vector3(7.0, 7.0),
                new Vector3(10.0, 15.0),
                new Vector3(15.0, 20.0),
                new Vector3(10.0, 20.0),
                new Vector3(5.0, 10.0),
            };
            var polygon = new Polygon(points).Simplify(0.5);
            Assert.Equal(3, polygon.Vertices.Count);
        }

        [Fact]
        public void Skeleton()
        {
            var skeleton = Shaper.L(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Skeleton();
            Assert.Equal(8, skeleton.Count);
            skeleton = Shaper.C(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Skeleton();
            Assert.Equal(11, skeleton.Count);
            skeleton = Shaper.X(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Skeleton();
            Assert.Equal(16, skeleton.Count);
        }

        [Fact]
        public void Spine()
        {
            var spine = Shaper.L(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Spine();
            Assert.Equal(2, spine.Count);
            spine = Shaper.C(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Spine();
            Assert.Equal(3, spine.Count);
            spine = Shaper.X(Vector3.Origin, new Vector3(100.0, 100.0), 25.0).Spine();
            Assert.Equal(4, spine.Count);
            var model = new Model();
            var corridors = new List<Polygon>();
            foreach (var line in spine)
            {
                corridors.Add(new Polyline(new[] { line.Start, line.End }).Offset(1.0, EndType.Square).First());
            }
            corridors = Shaper.Merge(corridors);
            foreach (var corridor in corridors)
            {
                model.AddElement(new Space(corridor, 4.0, new Material(Palette.Aqua, 0.0f, 0.0f, false, null, false, Guid.NewGuid(), "corridor")));
            }
            model.ToGlTF("../../../../testOutPut/spine.glb");
        }
    }
}
