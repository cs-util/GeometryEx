using System.Collections.Generic;
using System.Linq;
using Xunit;
using Elements;
using Elements.Geometry;
using Elements.Serialization.glTF;
using GeometryEx;

namespace GeometryExTests
{
    public class ShaperTests
    {
        [Fact]
        public void AdjacentArea()
        {
            var adjTo =
                new Polygon(
                    new[]
                    {
                        Vector3.Origin,
                        new Vector3(4.0, 0.0),
                        new Vector3(4.0, 4.0),
                        new Vector3(0.0, 4.0)
                    });
            var polygon = Shaper.AdjacentArea(adjTo, 20.0, Orient.N);
            Assert.Equal(20.0, polygon.Area());
            Assert.Contains(new Vector3(0.0, 4.0), polygon.Vertices);
            Assert.Contains(new Vector3(4.0, 4.0), polygon.Vertices);
            Assert.Contains(new Vector3(0.0, 9.0), polygon.Vertices);
            Assert.Contains(new Vector3(4.0, 9.0), polygon.Vertices);

            polygon = Shaper.AdjacentArea(adjTo, 20.0, Orient.S);
            Assert.Equal(20.0, polygon.Area());
            Assert.Contains(new Vector3(0.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(4.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(0.0, -5.0), polygon.Vertices);
            Assert.Contains(new Vector3(4.0, -5.0), polygon.Vertices);

            polygon = Shaper.AdjacentArea(adjTo, 20.0, Orient.W);
            Assert.Equal(20.0, polygon.Area());
            Assert.Contains(new Vector3(0.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(-5.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(-5.0, 4.0), polygon.Vertices);
            Assert.Contains(new Vector3(0.0, 4.0), polygon.Vertices);

            polygon = Shaper.AdjacentArea(adjTo, 20.0, Orient.E);
            Assert.Equal(20.0, polygon.Area());
            Assert.Contains(new Vector3(4.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(4.0, 4.0), polygon.Vertices);
            Assert.Contains(new Vector3(9.0, 0.0), polygon.Vertices);
            Assert.Contains(new Vector3(9.0, 4.0), polygon.Vertices);
        }

        [Fact]
        public void AxisQuad()
        {
            var polygon = new Polygon
            (
                new[]
                {
                    new Vector3(3.0, 1.0),
                    new Vector3(6.0, 1.0),
                    new Vector3(1.0, 6.0),
                    new Vector3(1.0, 3.0)
                }
            );
            var axis = Shaper.AxisQuad(polygon);
            var start = new Vector3(2.0, 2.0);
            var end = new Vector3(3.5, 3.5);

            Assert.Equal(axis.Start.X, start.X);
            Assert.Equal(axis.Start.Y, start.Y);
            Assert.Equal(axis.End.X, end.X);
            Assert.Equal(axis.End.Y, end.Y);
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
            polygon = polygon.ExpandtoArea(20.0, 0.1, Orient.C, within, among);
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
            model.ToGlTF("../../../../expandToArea.glb");
        }

        [Fact]
        public void CombinePolygons()
        {
            var polygon = new Polygon
            (
                new[]
                {
                    new Vector3(0.0, 0.0),
                    new Vector3(12.0, 0.0),
                    new Vector3(12.0, 12.0),
                    new Vector3(0.0, 12.0)
                }
            );
            var among = new List<Polygon>()
            {
                new Polygon(
                    new[]
                    {
                        new Vector3(0.0, 0.0),
                        new Vector3(7.0, 0.0),
                        new Vector3(7.0, 4.0),
                        new Vector3(0.0, 4.0)
                    }),
                new Polygon(
                    new[]
                    {
                        new Vector3(0.0, 6.0),
                        new Vector3(3.0, 6.0),
                        new Vector3(3.0, 12.0),
                        new Vector3(0.0, 12.0)
                    }
                 ),
                new Polygon(
                    new[]
                    {
                        new Vector3(3.0, 4.0),
                        new Vector3(7.0, 4.0),
                        new Vector3(7.0, 9.0),
                        new Vector3(3.0, 9.0)
                    }
                 ),
                new Polygon(
                    new[]
                    {
                        new Vector3(5.0, 9.0),
                        new Vector3(8.0, 9.0),
                        new Vector3(8.0, 12.0),
                        new Vector3(5.0, 12.0)
                    }
                 ),
                new Polygon(
                    new[]
                    {
                        new Vector3(7.0, 2.0),
                        new Vector3(12.0, 2.0),
                        new Vector3(12.0, 7.0),
                        new Vector3(7.0, 7.0)
                    }
                 ),
                new Polygon(
                    new[]
                    {
                        new Vector3(9.0, 7.0),
                        new Vector3(12.0, 7.0),
                        new Vector3(12.0, 12.0),
                        new Vector3(9.0, 12.0)
                    }
                 ),
            };
            var polygons = Polygon.Difference(new List<Polygon> { polygon }, among);
            Assert.Equal(4, polygons.Count);
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
            model.ToGlTF("../../../../FitTo.glb");
        }

        [Fact]
        public void FitWithin()
        {
            var within = Shaper.U(Vector3.Origin, new Vector3(40.0, 40.0), 10.0);
            var fit = Polygon.Rectangle(new Vector3(-10.0, 20.0), new Vector3(50.0, 30.0));
            var polygons = Shaper.FitWithin(fit, within);

            Assert.Equal(2, polygons.Count);

            var vertices = polygons.First().Vertices.ToList();
            vertices.AddRange(polygons.Last().Vertices);

            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 20.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 30.0);
            Assert.Contains(vertices, p => p.X == 10.0 && p.Y == 20.0);
            Assert.Contains(vertices, p => p.X == 10.0 && p.Y == 30.0);
            Assert.Contains(vertices, p => p.X == 30.0 && p.Y == 20.0);
            Assert.Contains(vertices, p => p.X == 30.0 && p.Y == 30.0);
            Assert.Contains(vertices, p => p.X == 40.0 && p.Y == 20.0);
            Assert.Contains(vertices, p => p.X == 40.0 && p.Y == 30.0);
        }
        
        [Fact]
        public void InQuadrant()
        {
            var polygons = new List<Polygon>
            {
                new Polygon
                (
                    new []
                    {
                        Vector3.Origin,
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 3.0),
                        new Vector3(0.0, 3.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(-5.0, 0.0),
                        new Vector3(-8.0, 0.0),
                        new Vector3(-8.0, 20.0),
                        new Vector3(-5.0, 20.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(-10.0, -1.0),
                        new Vector3(-20.0, -1.0),
                        new Vector3(-20.0, -3.0),
                        new Vector3(-10.0, -3.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(10.0, 0.0),
                        new Vector3(20.0, 0.0),
                        new Vector3(20.0, -3.0),
                        new Vector3(10.0, -3.0)
                    }
                )
            };
            Assert.Single(Shaper.InQuadrant(polygons, Quadrant.I));
            Assert.Single(Shaper.InQuadrant(polygons, Quadrant.II));
            Assert.Single(Shaper.InQuadrant(polygons, Quadrant.III));
            Assert.Single(Shaper.InQuadrant(polygons, Quadrant.IV));
        }

        [Fact]
        public void Merge()
        {
            var polygons = new List<Polygon>
            {
                new Polygon
                (
                    new []
                    {
                        Vector3.Origin,
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 3.0),
                        new Vector3(0.0, 3.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(5.0, 0.0),
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 20.0),
                        new Vector3(5.0, 20.0)
                    }
                ),
                new Polygon
                (
                    new []
                    {
                        new Vector3(10.0, 0.0),
                        new Vector3(20.0, 0.0),
                        new Vector3(20.0, 3.0),
                        new Vector3(10.0, 3.0)
                    }
                )
            };
            var merged = Shaper.Merge(polygons);
            Assert.Equal(2, merged.Count);
        }

        [Fact]
        public void NearPolygons()
        {
            var polygon =
                new Polygon
                (
                    new[]
                    {
                        Vector3.Origin,
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 3.0),
                        new Vector3(0.0, 3.0)
                    }
                );
            var nearPolygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(5.0, 0.0),
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 20.0),
                        new Vector3(5.0, 20.0)
                    }
                );
            var polygons = Shaper.NearPolygons(polygon, nearPolygon, true);
            Assert.Equal(32, polygons.Count);
        }

        [Fact]
        public void NonIntersecting()
        {
            var polygon =
                new Polygon
                (
                    new[]
                    {
                        Vector3.Origin,
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 3.0),
                        new Vector3(0.0, 3.0)
                    }
                );
            var nearPolygon =
                new Polygon
                (
                    new[]
                    {
                        new Vector3(5.0, 0.0),
                        new Vector3(8.0, 0.0),
                        new Vector3(8.0, 20.0),
                        new Vector3(5.0, 20.0)
                    }
                );
            var polygons = Shaper.NearPolygons(polygon, nearPolygon, true);
            polygons = Shaper.NonIntersecting(polygon, polygons);
            Assert.Equal(24, polygons.Count);
        }

        [Fact]
        public void C()
        {
            var polygon = Shaper.C(new Vector3(2.0, 2.0), new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 6.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 6.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 7.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 7.0);
        }

        [Fact]
        public void E()
        {
            var polygon = Shaper.E(new Vector3(2.0, 2.0), new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 6.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 6.0);
            Assert.Contains(vertices, p => p.X == 5.0 && p.Y == 7.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 7.0);
        }

        [Fact]
        public void F()
        {
            var polygon = Shaper.F(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 5.0);
        }

        [Fact]
        public void H()
        {
            var polygon = Shaper.H(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 5.0);
        }

        [Fact]
        public void L()
        {
            var polygon = Shaper.L(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 1.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 1.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 5.0);
        }

        [Fact]
        public void PolygonT()
        {
            var polygon = Shaper.T(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 4.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 4.0);
        }

        [Fact]
        public void U()
        {
            var polygon = Shaper.U(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 1.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 1.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 5.0);
        }

        [Fact]
        public void X()
        {
            var polygon = Shaper.X(Vector3.Origin, new Vector3(3.0, 5.0), 1.0);
            var vertices = polygon.Vertices;
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 0.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 3.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 2.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 5.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 3.0);
            Assert.Contains(vertices, p => p.X == 0.0 && p.Y == 2.0);
            Assert.Contains(vertices, p => p.X == 1.0 && p.Y == 2.0);
        }

        [Fact]
        public void RectangleByArea()
        {
            //var polygon = Shaper.RectangleByArea(136.5, 1.3877787807814457);

            var polygon = Shaper.RectangleByArea(9.0, 1.0);
            Assert.Equal(9.0, polygon.Area());
            Assert.Contains(polygon.Vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(polygon.Vertices, p => p.X == 0.0 && p.Y == 3.0);
            Assert.Contains(polygon.Vertices, p => p.X == 3.0 && p.Y == 3.0);
            Assert.Contains(polygon.Vertices, p => p.X == 3.0 && p.Y == 0.0);
        }

        [Fact]
        public void RectangleByRatio()
        {
            var polygon = Shaper.RectangleByRatio(2.0);

            Assert.Equal(2.0, polygon.Area());
            Assert.Contains(polygon.Vertices, p => p.X == 0.0 && p.Y == 0.0);
            Assert.Contains(polygon.Vertices, p => p.X == 1.0 && p.Y == 0.0);
            Assert.Contains(polygon.Vertices, p => p.X == 1.0 && p.Y == 2.0);
            Assert.Contains(polygon.Vertices, p => p.X == 0.0 && p.Y == 2.0);
        }
    }
}
