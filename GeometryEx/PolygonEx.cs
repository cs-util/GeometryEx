using System;
using System.Collections.Generic;
using System.Linq;
using Elements.Geometry;
using ClipperLib;

namespace GeometryEx
{
    public static class PolygonEx
    {
        /// <summary>
        /// The ratio of the longer side to the shorter side of the Polygon's bounding box.
        /// </summary>
        public static double AspectRatio(this Polygon polygon)
        {
            var box = polygon.Compass();
            return box.SizeX >= box.SizeY ? box.SizeX / box.SizeY : box.SizeY / box.SizeX;
        }

        /// <summary>
        /// Hypothesizes a centerline of an oblong rectangular Polygon by finding the midpoint of the two shortest sides and creating a line between them. If the two shortest sides share a point, skips a side and returns the line form the midpoint of the first and third sides in the list.
        /// </summary>
        /// <returns>
        /// A new Line.
        /// </returns>
        public static Line AxisQuad(this Polygon polygon)
        {
            var segments = new List<Line>(polygon.Segments().OrderBy(i => i.Length()));
            if (segments.Count() != 4)
            {
                return null;
            }
            if (segments[0].Start.Equals(segments[1].End) || segments[0].End.Equals(segments[1].Start))
            {
                if (segments[0].Start.Equals(segments[2].End) && segments[0].End.Equals(segments[2].Start))
                {
                    return new Line(segments[0].Midpoint(), segments[2].Midpoint());
                }
                else
                {
                    return new Line(segments[0].Midpoint(), segments[3].Midpoint());
                }
            }
            return new Line(segments[0].Midpoint(), segments[1].Midpoint());
        }

        /// <summary>
        /// Returns a CompassBox representation of the Polygon's bounding box.
        /// </summary>
        public static CompassBox Compass(this Polygon polygon)
        {
            return new CompassBox(polygon);
        }

        /// <summary>
        /// Returns a list of Vector3 points representig the corners of the Polygon's orthogonal bounding box.
        /// </summary>
        public static List<Vector3> BoxCorners(this Polygon polygon)
        {
            var box = new CompassBox(polygon);
            return new List<Vector3>
            {
                box.SW,
                box.SE,
                box.NE,
                box.NW
            };
        }

        /// <summary>
        /// Tests if the supplied Vector3 point is within this Polygon without coincidence with an edge when compared on a shared plane.
        /// </summary>
        /// <param name="point">The Vector3 point to compare to this Polygon.</param>
        /// <returns>
        /// Returns true if the supplied Vector3 point is within this Polygon when compared on a shared plane. Returns false if the Vector3 point is outside this Polygon or if the supplied Vector3 point is null.
        /// </returns>
        public static bool Contains(this Polygon polygon, Vector3 point)
        {
            if (point.IsNaN())
            {
                return false;
            }
            var thisPath = ToClipperPath(polygon);
   
            var intPoint = new IntPoint(point.X * scale, point.Y * scale);
            if (Clipper.PointInPolygon(intPoint, thisPath) != 1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tests if the supplied Vector3 point is within this Polygon or coincident with an edge when compared on a shared plane.
        /// </summary>
        /// <param name="point">The Vector3 point to compare to this Polygon.</param>
        /// <returns>
        /// Returns true if the supplied Vector3 point is within this Polygon or coincident with an edge when compared on a shared plane. Returns false if the supplied point is outside this Polygon, or if the supplied Vector3 point is null.
        /// </returns>
        public static bool Covers(this Polygon polygon, Vector3 point)
        {
            if (point.IsNaN())
            {
                return false;
            }
            var thisPath = ToClipperPath(polygon);
            var intPoint = new IntPoint(point.X * scale, point.Y * scale);
            if (Clipper.PointInPolygon(intPoint, thisPath) == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tests if the supplied Polygon is within this Polygon with or without edge coincident vertices when compared on a shared plane.
        /// </summary>
        /// <param name="polygon">The Polygon to compare to this Polygon.</param>
        /// <returns>
        /// Returns true if every vertex of the supplied Polygon is within this Polygon or coincident with an edge when compared on a shared plane. Returns false if any vertex of the supplied Polygon is outside this Polygon, or if the supplied Polygon is null.
        /// </returns>
        private static bool Covers(Polygon cover, Polygon polygon)
        {
            if (polygon == null)
            {
                return false;
            }
            var clipper = new Clipper();
            var solution = new List<List<IntPoint>>();
            clipper.AddPath(cover.ToClipperPath(), PolyType.ptSubject, true);
            clipper.AddPath(polygon.ToClipperPath(), PolyType.ptClip, true);
            clipper.Execute(ClipType.ctUnion, solution);
            if (solution.Count != 1)
            {
                return false;
            }
            return Math.Abs(solution.First().ToPolygon().Area() - cover.ToClipperPath().ToPolygon().Area()) <= 0.0001;
        }

        /// <summary>
        /// Constructs the geometric difference between this Polygon and the supplied Polygons.
        /// </summary>
        /// <param name="difPolys">The list of intersecting Polygons.</param>
        /// <returns>
        /// Returns a list of Polygons representing the subtraction of the supplied Polygons from this Polygon.
        /// Returns null if the area of this Polygon is entirely subtracted.
        /// Returns a list containing a representation of the perimeter of this Polygon if the two Polygons do not intersect.
        /// </returns>
        public static List<Polygon> Difference(this Polygon polygon, IList<Polygon> difPolys)
        {
            var thisPath = ToClipperPath(polygon);
            var polyPaths = new List<List<IntPoint>>();
            foreach (Polygon poly in difPolys)
            {
                polyPaths.Add(poly.ToClipperPath());
            }
            Clipper clipper = new Clipper();
            clipper.AddPath(thisPath, PolyType.ptSubject, true);
            clipper.AddPaths(polyPaths, PolyType.ptClip, true);
            var solution = new List<List<IntPoint>>();
            clipper.Execute(ClipType.ctDifference, solution);
            if (solution.Count == 0)
            {
                return null;
            }
            var polygons = new List<Polygon>();
            foreach (List<IntPoint> path in solution)
            {
                var vertices = path.Distinct().ToList();
                polygons.Add(ToPolygon(vertices));
            }
            return polygons;
        }

        /// <summary>
        /// Tests if the supplied Vector3 point is outside this Polygon when compared on a shared plane.
        /// </summary>
        /// <param name="point">The Vector3 point to compare to this Polygon.</param>
        /// <returns>
        /// Returns true if the supplied Vector3 point is outside this Polygon when compared on a shared plane or if the supplied Vector3 point is null.
        /// </returns>
        public static bool Disjoint(this Polygon polygon, Vector3 point)
        {
            if (point.IsNaN())
            {
                return true;
            }
            var thisPath = ToClipperPath(polygon);
            var intPoint = new IntPoint(point.X * scale, point.Y * scale);
            if (Clipper.PointInPolygon(intPoint, thisPath) != 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Provides a list of points within a polygon by searching along lines between Polygon vertices and the Polygon's Centroid.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static List<Vector3> FindInternalPoints(this Polygon polygon, double interval = 1.0)
        {
            var centroid = polygon.Centroid();
            var distPoints = polygon.Vertices.ToList().OrderByDescending(v => v.DistanceTo(centroid));
            var segments = polygon.Segments();
            var intPoints = new List<Vector3>();
            foreach (var point in distPoints)
            {
                var ray = new Line(centroid, point);
                var lines = ray.DivideByLength(interval);
                foreach (var line in lines)
                {
                    foreach (var segment in segments)
                    {
                        if (ray.Intersects2D(segment))
                        {
                            if (polygon.Contains(line.Start))
                            {
                                intPoints.Add(line.Start);
                            }
                            if (polygon.Contains(line.End))
                            {
                                intPoints.Add(line.End);
                            }
                        }
                    }
                }
            }
            return intPoints;
        }

        /// <summary>
        /// Tests whether a Polygon is covered by a Polygon perimeter and doesn't intersect with a list of Polygons.
        /// </summary>
        /// <param name="polygon">The Polygon to test.</param>
        /// <param name="perimeter">The covering perimeter.</param>
        /// <param name="among">The list of Polygons to check for intersection.</param>
        /// <returns>
        /// True if the Polygon is covered by the perimeter and does not intersect with any Polygon in the supplied list.
        /// </returns>
        public static bool Fits(this Polygon polygon, Polygon within = null, IList<Polygon> among = null)
        {
            if (within != null && !Covers(within, polygon))
            {
                return false;
            }
            return !polygon.Intersects(among);
        }

        /// <summary>
        /// Tests if any of the supplied Polygons share one or more areas with this Polygon when compared on a shared plane.
        /// </summary>
        /// <param name="polygons">The Polygon to compare with this Polygon.</param>
        /// <returns>
        /// Returns true if any of the supplied Polygons share one or more areas with this Polygon when compared on a shared plane or if the list of supplied Polygons is null. Returns false if the none of the supplied Polygons share an area with this Polygon or if the supplied list of Polygons is null.
        /// </returns>
        public static bool Intersects(this Polygon polygon, IList<Polygon> polygons)
        {
            if (polygons == null)
            {
                return false;
            }
            foreach (Polygon poly in polygons)
            {
                if (polygon.Intersects(poly))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates whether this polygon is configured clockwise.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns>True if the polygon is oriented clockwise.</returns>
        public static bool IsClockWise(this Polygon polygon)
        {
            // https://en.wikipedia.org/wiki/Shoelace_formula
            var sum = 0.0;
            for (int i = 0; i < polygon.Vertices.Count; i++)
            {
                var point = polygon.Vertices[i];
                var nextPoint = polygon.Vertices[(i + 1) % polygon.Vertices.Count];
                sum += (nextPoint.X - point.X) * (nextPoint.Y + point.Y);
            }
            return sum > 0;
        }

        /// <summary>
        /// Returns a new Polygon displaced along a 2D vector calculated between the supplied Vector3 points.
        /// </summary>
        /// <param name="polygon">Polygon instance to be copied.</param>
        /// <param name="from">Vector3 base point of the move.</param>
        /// <param name="to">Vector3 target point of the move.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon MoveFromTo(this Polygon polygon, Vector3 from, Vector3 to)
        {
            var t = new Transform();
            t.Move(new Vector3(to.X - from.X, to.Y - from.Y));
            return t.OfPolygon(polygon);
        }

        /// <summary>
        /// Returns a new Polygon rotated around a supplied Vector3 by the specified angle in degrees.
        /// </summary>
        /// <param name="pivot">The Vector3 base point of the rotation.</param>
        /// <param name="angle">The desired rotation angle in degrees.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon Rotate(this Polygon polygon, Vector3 pivot, double angle)
        {
            var theta = angle * (Math.PI / 180);
            var vertices = new List<Vector3>();
            foreach(Vector3 vertex in polygon.Vertices)
            {
                var rX = (Math.Cos(theta) * (vertex.X - pivot.X)) - (Math.Sin(theta) * (vertex.Y - pivot.Y)) + pivot.X;
                var rY = (Math.Sin(theta) * (vertex.X - pivot.X)) + (Math.Cos(theta) * (vertex.Y - pivot.Y)) + pivot.Y;
                vertices.Add(new Vector3(rX, rY));
            }
            return new Polygon(vertices.ToArray());
        }

        /// <summary>
        /// Tests if the supplied Vector3 point is coincident with an edge of this Polygon when compared on a shared plane.
        /// </summary>
        /// <param name="point">The Vector3 point to compare to this Polygon.</param>
        /// <returns>
        /// Returns true if the supplied Vector3 point coincides with an edge of this Polygon when compared on a shared plane. Returns false if the supplied Vector3 point is not coincident with an edge of this Polygon, or if the supplied Vector3 point is null.
        /// </returns>
        public static bool Touches(this Polygon polygon, Vector3 point)
        {
            if (point.IsNaN())
            {
                return false;
            }
            var thisPath = ToClipperPath(polygon);
            var intPoint = new IntPoint(point.X * scale, point.Y * scale);
            if (Clipper.PointInPolygon(intPoint, thisPath) != -1)
            {
                return false;
            }
            return true;
        }

        private const double scale = 1024.0;

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
            return new Polygon(p.Select(v => new Vector3(v.X / scale, v.Y / scale)).Distinct().ToArray());
        }
    }
}
