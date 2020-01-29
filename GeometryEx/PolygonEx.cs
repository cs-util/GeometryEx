using System;
using System.Collections.Generic;
using System.Linq;
using Elements.Geometry;
using ClipperLib;

namespace GeometryEx
{
    public static class PolygonEx
    {
        public const double scale = 1024.0;

        /// <summary>
        /// The ratio of the longer side to the shorter side of the Polygon's bounding box.
        /// </summary>
        public static double AspectRatio(this Polygon polygon)
        {
            var box = polygon.Compass();
            return box.SizeX >= box.SizeY ? box.SizeX / box.SizeY : box.SizeY / box.SizeX;
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
            var thisPath = Shaper.ToClipperPath(polygon);
   
            var intPoint = new IntPoint(point.X * Shaper.scale, point.Y * scale);
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
            var thisPath = Shaper.ToClipperPath(polygon);
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
            var thisPath = Shaper.ToClipperPath(polygon);
            var intPoint = new IntPoint(point.X * scale, point.Y * scale);
            if (Clipper.PointInPolygon(intPoint, thisPath) != 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Attempts to scale up a Polygon until coming within the tolerance percentage of the target area.
        /// </summary>
        /// <param name="polygon">Polygon to scale.</param>
        /// <param name="area">Target area of the Polygon.</param>
        /// <param name="tolerance">Area total tolerance.</param>
        /// <param name="origin">Alignment location for final Polygon.</param>
        /// <param name="within">Polygon acting as a constraining outer boundary.</param>
        /// <param name="among">Llist of Polygons to avoid intersecting.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon ExpandtoArea(this Polygon polygon, double area,
                                           double tolerance = 0.1, Orient origin = Orient.C,
                                           Polygon within = null, List<Polygon> among = null)
        {
            if (polygon.IsClockWise())
            {
                polygon = polygon.Reversed();
            }
            if (Math.Abs(polygon.Area() - area) <= tolerance * area)
            {
                return polygon;
            }
            var pBox = polygon.Compass();
            var position = pBox.PointBy(origin);
            Polygon tryPoly = Polygon.Rectangle(Vector3.Origin, new Vector3(1.0, 1.0));
            double tryArea;
            do
            {
                tryArea = tryPoly.Area();
                double factor;
                if (tryArea >= area)
                {
                    factor = Math.Sqrt(tryArea / area);
                }
                else
                {
                    factor = Math.Sqrt(area / tryArea);
                }
                var t = new Transform();
                t.Scale(new Vector3(factor, factor));
                tryPoly = t.OfPolygon(tryPoly);
                var tBox = tryPoly.Compass();
                var from = tBox.PointBy(origin);
                tryPoly = tryPoly.MoveFromTo(from, position);
                if (within != null && tryPoly.Intersects(within))
                {
                    var tryPolys = within.Intersection(tryPoly);
                    if (tryPolys.Count > 0)
                    {
                        tryPoly = tryPolys.First();
                    }
                }
                if (among != null && tryPoly.Intersects(among))
                {
                    var tryPolys = tryPoly.Difference(among);
                    if (tryPolys.Count > 0)
                    {
                        tryPoly = tryPolys.First();
                    }
                }
            }
            while ((!Shaper.NearEqual(tryPoly.Area(), area, tolerance)) &&
                    !Shaper.NearEqual(tryPoly.Area(), tryArea, tolerance));
            return tryPoly;
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
        /// Creates the largest Polygon fitted within a supplied perimeter and conforming to supplied intersecting Polygons.
        /// </summary>
        /// <param name="polygon">This Polygon.</param>
        /// <param name="within">Polygon acting as a constraining outer boundary.</param>
        /// <param name="among">List of Polygons against which this Polygon must conform.</param>
        /// <returns>
        /// A list of Polygons.
        /// </returns>
        public static Polygon FitTo(this Polygon polygon, Polygon within, List<Polygon> among)
        {
            polygon = FitWithin(polygon, within);
            if (polygon == null)
            {
                return null;
            }
            return FitAmong(polygon, among);
        }

        /// <summary>
        /// Creates the largest Polygon fitted to supplied intersecting Polygons.
        /// </summary>
        /// <param name="polygon">This Polygon.</param>
        /// <param name="among">List of Polygons against which this Polygon must conform.</param>
        /// <returns>
        /// A Polygons.
        /// </returns>
        public static Polygon FitAmong(this Polygon polygon, List<Polygon> among = null)
        {
            var polyAmong = new List<Polygon>();
            polygon = Shaper.Difference(polygon, among);
            if (polygon.IsClockWise())
            {
                polygon.Reversed();
            }
            return polygon;
        }

        /// <summary>
        /// Creates the largest Polygon fitted within a supplied intersecting perimeter.
        /// </summary>
        /// <param name="polygon">This Polygon.</param>
        /// <param name="within">Polygon acting as a constraining outer boundary.</param>
        /// <returns>
        /// A Polygon.
        /// </returns>
        public static Polygon FitWithin(this Polygon polygon, Polygon within)
        {
            if (!within.Intersects(polygon))
            {
                return null;
            }
            var polygons = within.Intersection(polygon);
            if (polygons.Count == 0)
            {
                return null;
            }
            polygons = polygons.OrderByDescending(p => Math.Abs(p.Area())).ToList();
            if (polygons.First().IsClockWise())
            {
                return polygons.First().Reversed();
            }
            return polygons.First();
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
        /// <param name="polygon">This Polygon.</param>
        /// <param name="from">Vector3 base point of the move.</param>
        /// <param name="to">Vector3 target point of the move.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon MoveFromTo(this Polygon polygon, Vector3 from, Vector3 to)
        {
            var t = new Transform();
            t.Move(new Vector3(to.X - from.X, to.Y - from.Y, to.Z - from.Z));
            return t.OfPolygon(polygon);
        }

        /// <summary>
        /// Returns a new Polygon placed in a spatial relationship with a supplied polygon by using supplied Orient points derived from Polygon.Compass bounding box points on each Polygon.
        /// </summary>
        /// <param name="polygon">This Polygon.</param>
        /// <param name="adjTo">Reference Polygon in relation to which to place This Polygon.</param>
        /// <param name="from">Orient value indicating the point on This Polygon to use a MoveFromTo 'from' value.</param>
        /// <param name="to">Orient value indicating the point on the reference Polygon as a MoveFromTo 'to' value.</param>
        /// <returns>A new Polygon.</returns>
        public static Polygon PlaceNear(this Polygon polygon, Polygon adjTo, Orient from, Orient to)
        {
            var thisCompass = polygon.Compass();
            var adjCompass = adjTo.Compass();
            return polygon.MoveFromTo(thisCompass.PointBy(from), adjCompass.PointBy(to));
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
        /// Creates a new CCW Polygon of the same vertices with the start point now at the specified Polygon vertex.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="startFrom">The point from which to start the new Polygon.</param>
        /// <returns>A Polygon.</returns>
        public static Polygon RewindFrom(this Polygon polygon, Vector3 startFrom)
        {
            var vertices = polygon.Vertices;
            if (!vertices.Contains(startFrom))
            {
                return null;
            }
            var points = new List<Vector3>();
            var start = vertices.IndexOf(startFrom);
            points.Add(vertices[start]);
            var i = start + 1;
            while (i < vertices.Count)
            {
                points.Add(vertices[i]);
                i++;
            }
            i = 0;
            while (i < start)
            {
                points.Add(vertices[i]);
                i++;
            }
            return new Polygon(points);
        }

        /// <summary>
        /// Creates a new CCW Polygon of the same vertices with the start point now at the indexed Polygon vertex.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="start">The index of the point from which to start the new Polygon.</param>
        /// <returns>A Polygon.</returns>
        public static Polygon RewindFrom(this Polygon polygon, int start)
        {
            var vertices = polygon.Vertices;
            if (start > vertices.Count - 1)
            {
                return null;
            }
            var points = new List<Vector3>();
            points.Add(vertices[start]);
            var i = start + 1;
            while (i < vertices.Count)
            {
                points.Add(vertices[i]);
                i++;
            }
            i = 0;
            while (i < start)
            {
                points.Add(vertices[i]);
                i++;
            }
            return new Polygon(points);
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
            var thisPath = Shaper.ToClipperPath(polygon);
            var intPoint = new IntPoint(point.X * scale, point.Y * scale);
            if (Clipper.PointInPolygon(intPoint, thisPath) != -1)
            {
                return false;
            }
            return true;
        }
    }
}
