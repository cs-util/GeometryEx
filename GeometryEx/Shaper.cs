using System;
using System.Collections.Generic;
using ClipperLib;
using System.Linq;
using Elements.Geometry;

namespace GeometryEx
{
    /// <summary>
    /// Utilities for creating and editing Polygons.
    /// </summary>
    public static class Shaper
    {
        /// <summary>
        /// Creates a rectilinear Polygon in the specified adjacent quadrant to the supplied Polygon's bounding box.
        /// </summary>    
        /// <param name="area">The area of the new Polygon.</param>
        /// <param name="orient">The relative cardinal direction in which the new Polygon will be placed.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon AdjacentArea(Polygon polygon, double area, Orient orient)
        {
            var box = new CompassBox(polygon);
            double sizeX = 0.0;
            double sizeY = 0.0;
            if (orient == Orient.N || orient == Orient.S)
            {
                sizeX = box.SizeX;
                sizeY = area / box.SizeX;
            }
            else
            {
                sizeX = area / box.SizeY;
                sizeY = box.SizeY;
            }
            Vector3 origin = Vector3.Origin;
            switch (orient)
            {
                case Orient.N:
                    origin = box.NW;
                    break;
                case Orient.E:
                    origin = box.SE;
                    break;
                case Orient.S:
                    origin = new Vector3(box.SW.X, box.SW.Y - sizeY);
                    break;
                case Orient.W:
                    origin = new Vector3(box.SW.X - sizeX, box.SW.Y);
                    break;
            }
            return
                new Polygon
                (
                    new[]
                    {
                        origin,
                        new Vector3(origin.X + sizeX, origin.Y),
                        new Vector3(origin.X + sizeX, origin.Y + sizeY),
                        new Vector3(origin.X, origin.Y + sizeY)
                    }
                );
        }

        /// <summary>
        /// Hypothesizes a centerline of a rectangular Polygon by finding the midpoint of the shortest side and creating a line between its midpoint and midpoint of the second segment away from that side.
        /// </summary>
        /// <returns>
        /// A new Line.
        /// </returns>
        public static Line AxisQuad(Polygon polygon)
        {
            var segments = polygon.Segments();
            if (segments.Count() != 4)
            {
                throw new ArgumentException("Polygon must have 4 sides");
            }
            var shortest = segments.OrderBy(s => s.Length()).ToArray()[0];
            var points = polygon.Vertices.ToList();
            points.Remove(shortest.Start);
            points.Remove(shortest.End);
            return new Line(shortest.Midpoint(), new Line(points.First(), points.Last()).Midpoint());
        }

        /// <summary>
        /// Constructs the largest geometric difference between this Polygon and the supplied Polygons.
        /// </summary>
        /// <param name="difPolys">The list of intersecting Polygons.</param>
        /// <returns>
        /// Returns a Polygon representing the subtraction of the supplied Polygons from this Polygon or null if the area of this Polygon is entirely subtracted.
        /// </returns>
        public static Polygon Difference(Polygon polygon, IList<Polygon> difPolys)
        {
            foreach (Polygon differ in difPolys)
            {
                var thisPath = polygon.ToClipperPath();
                var clipper = new Clipper();
                clipper.AddPath(thisPath, PolyType.ptSubject, true);
                clipper.AddPath(differ.ToClipperPath(), PolyType.ptClip, true);
                var solution = new List<List<IntPoint>>();
                clipper.Execute(ClipType.ctDifference, solution);
                if (solution.Count == 0)
                {
                    // polygon has disappeared into a larger polygon.
                    return null;
                }
                var polygons = new List<Polygon>();
                foreach (List<IntPoint> path in solution)
                {
                    polygon = ToPolygon(path.Distinct().ToList());
                    if (polygon == null)
                    {
                        continue;
                    }
                    polygons.Add(polygon);
                }
                polygon = polygons.OrderByDescending(p => Math.Abs(p.Area())).First();
            }
            if (polygon.IsClockWise())
            {
                return polygon.Reversed();
            }
            return polygon;
        }

        /// <summary>
        /// Constructs the geometric differences between this Polygon and the supplied Polygons.
        /// </summary>
        /// <param name="difPolys">The list of intersecting Polygons.</param>
        /// <returns>
        /// Returns a list of Polygons representing the subtraction of the supplied Polygons from this Polygon or null if the area of this Polygon is entirely subtracted.
        /// </returns>
        public static List<Polygon> Differences(Polygon polygon, IList<Polygon> difPolys)
        {
            var polygons = new List<Polygon>();
            foreach (Polygon differ in difPolys)
            {
                var thisPath = polygon.ToClipperPath();
                var clipper = new Clipper();
                clipper.AddPath(thisPath, PolyType.ptSubject, true);
                clipper.AddPath(differ.ToClipperPath(), PolyType.ptClip, true);
                var solution = new List<List<IntPoint>>();
                clipper.Execute(ClipType.ctDifference, solution);
                if (solution.Count == 0)
                {
                    // polygon has disappeared into a larger polygon.
                    return null;
                }
                foreach (List<IntPoint> path in solution)
                {
                    polygon = ToPolygon(path.Distinct().ToList());
                    if (polygon == null)
                    {
                        continue;
                    }
                    if (polygon.IsClockWise())
                    {
                        polygon = polygon.Reversed();
                    }
                    polygons.Add(polygon);
                }
            }
            return polygons.OrderByDescending(p => Math.Abs(p.Area())).ToList();
        }

        /// <summary>
        /// Constructs the geometric union of the supplied list of Polygons.
        /// </summary>
        /// <param name="polygons">The list of Polygons to be combined.</param>
        /// <returns>
        /// List of Polygons.
        /// </returns>
        public enum FillType { EvenOdd, NonZero, Positive, Negative };
        public static List<Polygon> Merge(List<Polygon> polygons, FillType fillType = FillType.NonZero)
        {
            var filtyp = (PolyFillType) fillType;
            var polyPaths = new List<List<IntPoint>>();
            foreach (Polygon polygon in polygons)
            {
                polyPaths.Add(polygon.ToClipperPath());
            }
            Clipper clipper = new Clipper();
            clipper.AddPaths(polyPaths, PolyType.ptClip, true);
            clipper.AddPaths(polyPaths, PolyType.ptSubject, true);
            var solution = new List<List<IntPoint>>();
            clipper.Execute(ClipType.ctUnion, solution, filtyp);
            if (solution.Count == 0)
            {
                return polygons;
            }
            var mergePolygons = new List<Polygon>();
            foreach (var solved in solution)
            {
                var polygon = solved.Distinct().ToList().ToPolygon();
                if (polygon.IsClockWise())
                {
                    polygon = polygon.Reversed();
                }
                mergePolygons.Add(polygon);
            }
            return mergePolygons;
        }


        /// <summary>
        /// Creates an C-shaped Polygon within a specified rectangle with its southwest corner at the origin.
        /// </summary>
        /// <param name="origin">The southwest enclosing box corner.</param>
        /// <param name="size">The positive x and y delta defining the size of the enclosing box.</param>
        /// <param name="width">Width of each stroke of the shape.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon PolygonC(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width >= size.X || width * 3 >= size.Y)
            {
                throw new ArgumentOutOfRangeException(Messages.POLYGON_SHAPE_EXCEPTION);
            }
            return
                new Polygon
                (
                    new[]
                    {
                        Vector3.Origin,
                        new Vector3(size.X, Vector3.Origin.Y),
                        new Vector3(size.X, width),
                        new Vector3(width, width),
                        new Vector3(width, size.Y - width),
                        new Vector3(size.X, size.Y - width),
                        new Vector3(size.X, size.Y),
                        new Vector3(Vector3.Origin.X, size.Y),
                    }
                ).MoveFromTo(Vector3.Origin, origin);
        }

        /// <summary>
        /// Creates an E-shaped Polygon within a specified rectangle.
        /// </summary>
        /// <param name="origin">The southwest enclosing box corner.</param>
        /// <param name="size">The positive x and y delta defining the size of the enclosing box.</param>
        /// <param name="width">Width of each stroke of the shape.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon PolygonE(Vector3 origin, Vector3 size, double width)
        {
            if(size.X <= 0 || size.Y <= 0 || width >= size.X || width * 3 >= size.Y)
            {
                throw new ArgumentOutOfRangeException(Messages.POLYGON_SHAPE_EXCEPTION);
            }
            var halfWidth = width * 0.5;
            var xAxis = size.Y * 0.5;
            return 
                new Polygon
                (
                    new []
                    {
                        Vector3.Origin,
                        new Vector3(size.X, Vector3.Origin.Y),
                        new Vector3(size.X, width),
                        new Vector3(width, width),
                        new Vector3(width, xAxis - halfWidth),//
                        new Vector3(size.X, xAxis - halfWidth),
                        new Vector3(size.X, xAxis + halfWidth),
                        new Vector3(width, xAxis + halfWidth),
                        new Vector3(width, size.Y - width),//
                        new Vector3(size.X, size.Y - width),
                        new Vector3(size.X, size.Y),
                        new Vector3(Vector3.Origin.X, size.Y),
                    }
                ).MoveFromTo(Vector3.Origin, origin);
        }

        /// <summary>
        /// Creates an F-shaped Polygon within a specified rectangle.
        /// </summary>
        /// <param name="origin">The initial enclosing box corner.</param>
        /// <param name="size">The positive x and y delta defining the size of the enclosing box.</param>
        /// <param name="width">Width of each stroke of the shape.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon PolygonF(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width >= size.X || width * 2 >= size.Y)
            {
                throw new ArgumentOutOfRangeException(Messages.POLYGON_SHAPE_EXCEPTION);
            }
            var halfWidth = width * 0.5;
            var xAxis = size.Y * 0.5;
            return
                new Polygon
                (
                    new []
                    {
                        Vector3.Origin,
                        new Vector3(width, Vector3.Origin.Y),
                        new Vector3(width, xAxis - halfWidth),
                        new Vector3(size.X, xAxis - halfWidth),
                        new Vector3(size.X, xAxis + halfWidth),
                        new Vector3(width, xAxis + halfWidth),
                        new Vector3(width, size.Y - width),
                        new Vector3(size.X, size.Y - width),
                        new Vector3(size.X, size.Y),
                        new Vector3(Vector3.Origin.X, size.Y),
                    }
                ).MoveFromTo(Vector3.Origin, origin);
        }

        /// <summary>
        /// Creates an H-shaped Polygon within a specified rectangle.
        /// </summary>
        /// <param name="origin">The initial enclosing box corner.</param>
        /// <param name="size">The positive x and y delta defining the size of the enclosing box.</param>
        /// <param name="width">Width of each stroke of the shape.</param>
        /// <param name="offset">Positive or negative displacement of the H crossbar from the shape meridian.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon PolygonH(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width * 2 >= size.X || width >= size.Y)
            {
                throw new ArgumentOutOfRangeException(Messages.POLYGON_SHAPE_EXCEPTION);
            }
            var halfWidth = width * 0.5;
            var xAxis = size.Y * 0.5;
            var rightWest = size.X - width;
            return 
                new Polygon
                (
                    new []
                    {
                        Vector3.Origin,
                        new Vector3(width, Vector3.Origin.Y),
                        new Vector3(width, xAxis - halfWidth),
                        new Vector3(rightWest, xAxis - halfWidth),
                        new Vector3(rightWest, Vector3.Origin.Y),
                        new Vector3(size.X, Vector3.Origin.Y),
                        new Vector3(size.X, size.Y),
                        new Vector3(rightWest, size.Y),
                        new Vector3(rightWest, xAxis + halfWidth),
                        new Vector3(width, xAxis + halfWidth),
                        new Vector3(width, size.Y),
                        new Vector3(Vector3.Origin.X, size.Y),
                    }
                ).MoveFromTo(Vector3.Origin, origin);
        }

        /// <summary>
        /// Creates an L-shaped Polygon within a specified rectangle.
        /// </summary>
        /// <param name="origin">The initial enclosing box corner.</param>
        /// <param name="size">The positive x and y delta defining the size of the enclosing box.</param>
        /// <param name="width">Width of each stroke of the shape.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon PolygonL(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width >= size.X || width >= size.Y)
            {
                throw new ArgumentOutOfRangeException(Messages.POLYGON_SHAPE_EXCEPTION);
            }
            return
                new Polygon
                (
                    new[]
                    {
                        Vector3.Origin,
                        new Vector3(size.X, Vector3.Origin.Y),
                        new Vector3(size.X, width),
                        new Vector3(width, width),
                        new Vector3(width, size.Y),
                        new Vector3(Vector3.Origin.X, size.Y)
                    }
                ).MoveFromTo(Vector3.Origin, origin);
        }

        /// <summary>
        /// Creates a T-shaped Polygon within a specified rectangle.
        /// </summary>
        /// <param name="origin">The initial enclosing box corner.</param>
        /// <param name="size">The positive x and y delta defining the size of the enclosing box.</param>
        /// <param name="width">Width of each stroke of the shape.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon PolygonT(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width >= size.X || width >= size.Y)
            {
                throw new ArgumentOutOfRangeException(Messages.POLYGON_SHAPE_EXCEPTION);
            }
            var halfWidth = width * 0.5;
            var yAxis = origin.X + (size.X * 0.5);
            return
                new Polygon
                (
                    new []
                    {
                        new Vector3(yAxis - halfWidth, 0),
                        new Vector3(yAxis + halfWidth, 0),
                        new Vector3(yAxis + halfWidth, size.Y - width),
                        new Vector3(size.X, size.Y - width),
                        new Vector3(size.X, size.Y),
                        new Vector3(Vector3.Origin.X, size.Y),
                        new Vector3(Vector3.Origin.X, size.Y - width),
                        new Vector3(yAxis - halfWidth, size.Y - width)
                    }
                ).MoveFromTo(Vector3.Origin, origin);
        }

        /// <summary>
        /// Creates U-shaped Polygon within a specified rectangle.
        /// </summary>
        /// <param name="origin">The initial enclosing box corner.</param>
        /// <param name="size">The positive x and y delta defining the size of the enclosing box.</param>
        /// <param name="width">Width of each stroke of the shape.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon PolygonU(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width * 2 >= size.X || width >= size.Y)
            {
                throw new ArgumentOutOfRangeException(Messages.POLYGON_SHAPE_EXCEPTION);
            }
            return
                new Polygon
                (
                    new []
                    {
                        Vector3.Origin,
                        new Vector3(size.X, Vector3.Origin.Y),
                        new Vector3(size.X, Vector3.Origin.Y + size.Y),
                        new Vector3(size.X - width, Vector3.Origin.Y + size.Y),
                        new Vector3(size.X - width, Vector3.Origin.Y + width),
                        new Vector3(width, Vector3.Origin.Y + width),
                        new Vector3(width, Vector3.Origin.Y + size.Y),
                        new Vector3(Vector3.Origin.X, Vector3.Origin.Y + size.Y)
                    }
                ).MoveFromTo(Vector3.Origin, origin);
        }

        /// <summary>
        /// Creates an X-shaped Polygon within a specified rectangle.
        /// </summary>
        /// <param name="origin">The initial enclosing box corner.</param>
        /// <param name="size">The positive x and y delta defining the size of the enclosing box.</param>
        /// <param name="width">Width of each stroke of the shape.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon PolygonX(Vector3 origin, Vector3 size, double width)
        {
            if (width >= Math.Abs(size.X - origin.X) || width >= Math.Abs(size.Y - origin.Y))
            {
                throw new ArgumentOutOfRangeException(Messages.POLYGON_SHAPE_EXCEPTION);
            }
            var halfWidth = width * 0.5;
            var xAxis = origin.Y + (size.Y * 0.5);
            var yAxis = origin.X + (size.X * 0.5);
            return 
                new Polygon
                (               
                    new []
                    {
                        new Vector3(yAxis - halfWidth, Vector3.Origin.Y),
                        new Vector3(yAxis + halfWidth, Vector3.Origin.Y),
                        new Vector3(yAxis + halfWidth, xAxis - halfWidth),
                        new Vector3(size.X, xAxis - halfWidth),
                        new Vector3(size.X, xAxis + halfWidth),
                        new Vector3(yAxis + halfWidth, xAxis + halfWidth),
                        new Vector3(yAxis + halfWidth, size.Y),
                        new Vector3(yAxis - halfWidth, size.Y),
                        new Vector3(yAxis - halfWidth, xAxis + halfWidth),
                        new Vector3(Vector3.Origin.X, xAxis + halfWidth),
                        new Vector3(Vector3.Origin.X, xAxis - halfWidth),
                        new Vector3(yAxis - halfWidth, xAxis - halfWidth)
                    }
                ).MoveFromTo(Vector3.Origin, origin);
        }

        /// <summary>
        /// Creates a rectangular Polygon of the supplied length to width proportion at the supplied area with its southwest corner at the origin.
        /// </summary>
        /// <param name="area">Required area of the Polygon.</param>
        /// <param name="ratio">Ratio of width to depth.</param>
        /// <param name="moveTo">Location of the southwest corner of the new Polygon.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon RectangleByArea(double area, double ratio = 1.0)
        {
            if (area <= 0.0 || ratio <= 0.0)
            {
                throw new ArgumentOutOfRangeException(Messages.POLYGON_SHAPE_EXCEPTION);
            }
            return Polygon.Rectangle(Vector3.Origin, new Vector3(Math.Sqrt(area * ratio), area / Math.Sqrt(area * ratio)));
        }

        /// <summary>
        /// Creates a rectangular Polygon of the supplied length to width proportion at the supplied area with its southwest corner at the origin.
        /// </summary>
        /// <param name="ratio">Ratio of width to depth.</param>
        /// <param name="moveTo">Location of the southwest corner of the new Polygon.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon RectangleByRatio(double ratio = 1.0)
        {
            if (ratio <= 0.0)
            {
                throw new ArgumentOutOfRangeException(Messages.POLYGON_SHAPE_EXCEPTION);
            }
            return Polygon.Rectangle(Vector3.Origin, new Vector3(1.0, ratio));
        }

        /// <summary>
        /// Constructs a list of line segments in order from pairs in a list of vertices.
        /// </summary>
        /// <param name="vertices">List of vertices to convert to line segments.</param>
        /// <returns>List of Lines.</returns>
        public static List<Line> LinesFromPoints(List<Vector3> points)
        {
            var lines = new List<Line>();
            for (var i = 0; i < points.Count - 1; i++)
            {
                lines.Add(new Line(points[i], points[i + 1]));
            }
            return lines;
        }

        /// <summary>
        /// Check if any of lines have zero length.
        /// </summary>
        public static bool ZeroLength(List<Line> lines)
        {
            foreach (var s in lines)
            {
                if (s.Length() == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check for self-intersection in the supplied lines.
        /// </summary>
        /// <param name="lines">List of lines to check.</param>
        public static bool Intersects(List<Line> lines)
        {
            for (var i = 0; i < lines.Count; i++)
            {
                for (var j = 0; j < lines.Count; j++)
                {
                    if (i == j)
                    {
                        // Don't check against itself.
                        continue;
                    }
                    if (lines[i].Intersects2D(lines[j]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Tests if two doubles are effectively equal within a tolerance.
        /// </summary>
        /// <param name="thisValue">The lower bound of the random range.</param>
        /// <param name="thatValue">The upper bound of the random range.</param>
        /// <returns>
        /// True if the supplied values are equivalent within the default or supplied tolerance.
        /// </returns>
        public static bool NearEqual(this double thisValue, double thatValue, double tolerance = 1e-9)
        {
            if (Math.Abs(thisValue - thatValue) > tolerance)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a random double within the supplied range.
        /// </summary>
        /// <param name="minValue">The lower bound of the random range.</param>
        /// <param name="minValue">The upper bound of the random range.</param>
        /// <returns>
        /// A random double within the range.
        /// </returns>
        public static double RandomDouble(double minvalue, double maxvalue)
        {
            var scale = 1000000.0;
            var rnd = new Random();
            double next = rnd.Next((int)Math.Round(minvalue * scale), (int)Math.Round(maxvalue * scale));
            return next / scale;
        }

        public const double scale = 1024.0;

        /// <summary>
        /// Construct a clipper path from a Polygon.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static List<IntPoint> ToClipperPath(this Polygon p)
        {
            var path = new List<IntPoint>();
            foreach (var v in p.Vertices)
            {
                path.Add(new IntPoint(v.X * scale, v.Y * scale));
            }
            return path.Distinct().ToList();
        }

        /// <summary>
        /// Construct a Polygon from a clipper path 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static Polygon ToPolygon(this List<IntPoint> p)
        {
            var points = p.Select(v => new Vector3(v.X / scale, v.Y / scale)).Distinct().ToList();
            var lines = LinesFromPoints(points);
            if (ZeroLength(lines) || Intersects(lines))
            {
                return null;
            }
            return new Polygon(points);
        }
    }
}
