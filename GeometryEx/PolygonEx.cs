using System;
using System.Collections.Generic;
using System.Linq;
using Elements.Geometry;
using StraightSkeletonNet;
using StraightSkeletonNet.Primitives;

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
        public static Polygon ExpandToArea(this Polygon polygon, double area, double ratio,
                                           double tolerance = 0.1, Orient origin = Orient.C,
                                           Polygon within = null, List<Polygon> among = null)
        {
            if (polygon.IsClockWise())
            {
                polygon = polygon.Reversed();
            }
            if (Math.Abs(polygon.Area() - area) <= Math.Abs(tolerance * area))
            {
                return polygon;
            }
            var position = polygon.Compass().PointBy(origin);
            Polygon tryPoly = Shaper.RectangleByArea(area, ratio);
            tryPoly = tryPoly.MoveFromTo(tryPoly.Compass().PointBy(origin), position);
            double tryArea = tryPoly.Area();
            do
            {
                var t = new Transform();
                t.Scale(tryArea / area, tryPoly.Compass().PointBy(origin));
                tryPoly = t.OfPolygon(tryPoly);
                if (within != null && tryPoly.Intersects(within))
                {
                    var tryPolys = within.Intersection(tryPoly);
                    if (tryPolys != null && tryPolys.Count > 0)
                    {
                        tryPoly = tryPolys.First();
                    }
                }
                if (among != null && tryPoly.Intersects(among))
                {
                    var tryPolys = tryPoly.Difference(among);
                    if (tryPolys != null && tryPolys.Count > 0)
                    {
                        tryPoly = tryPolys.First();
                    }
                }
                tryArea = tryPoly.Area();
            }
            while (!Shaper.NearEqual(tryPoly.Area(), area, tolerance * area) &&
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
            if (within != null && !within.Covers(polygon))
            {
                return false;
            }
            return !polygon.Intersects(among);
        }

        /// <summary>
        /// Creates the largest Polygon fitted to supplied intersecting Polygons.
        /// </summary>
        /// <param name="polygon">This Polygon.</param>
        /// <param name="among">List of Polygons against which this Polygon must conform.</param>
        /// <returns>
        /// A Polygons.
        /// </returns>
        public static Polygon FitAmong(this Polygon polygon, List<Polygon> among)
        {
            if (among == null)
            {
                return null;
            }
            var polygons = Polygon.Difference(new List<Polygon>() { polygon }, among);
            if (polygons == null || polygons.Count == 0)
            {
                return null;
            }
            polygon = polygons.OrderByDescending(p => Math.Abs(p.Area())).First();
            if (polygon.IsClockWise())
            {
                return polygon.Reversed();
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
        public static Polygon FitMost(this Polygon polygon, Polygon within)
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
            polygon = FitMost(polygon, within);
            if (polygon == null)
            {
                return null;
            }
            return FitAmong(polygon, among);
        }

        /// <summary>
        /// Tests if any of the supplied Polygons share one or more areas with this Polygon when compared on a shared plane.
        /// </summary>
        /// <param name="polygons">The Polygon to compare with this Polygon.</param>
        /// <returns>
        /// Returns true if any of the supplied Polygons share one or more areas with this Polygon when compared on a shared plane or if the list of supplied Polygons is null. Returns false if the none of the supplied Polygons share an area with this Polygon or if the supplied list of Polygons is null.
        /// </returns>
        /// TODO: Move to ELEMENTS
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
            var t = new Transform(new Vector3(to.X - from.X, to.Y - from.Y, to.Z - from.Z));
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
                var rZ = vertex.Z;
                vertices.Add(new Vector3(rX, rY, rZ));
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
            if (start < 0 || start > vertices.Count - 1)
            {
                return null;
            }
            var points = new List<Vector3>()
            {
                vertices[start]
            };
            var i = start + 1;
            while (i < vertices.Count)
            {
                points.Add(vertices[i % vertices.Count]);
                i++;
            }
            return new Polygon(points);
        }

        /// <summary>
        /// Reduces Polygon vertices.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static Polygon Simplify(this Polygon polygon, double tolerance)
        {
            var points = Shaper.Simplify(polygon.Vertices.ToList(), tolerance);
            if (points.Count < 3)
            {
                return polygon;
            }
            return new Polygon(points);
        }

        /// <summary>
        /// Returns a List of Lines forming the Polygon skeleton's connections between Polygon's spine and vertcies.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns>A List of Lines</Line></returns>
        public static List<Line> Ribs(this Polygon polygon)
        {
            var skeleton = polygon.Skeleton();
            var lines = new List<Line>();
            foreach (var line in skeleton)
            {
                if (polygon.Contains(line.Start) ^ polygon.Contains(line.End))
                {
                    lines.Add(line);
                }
            }
            return lines.OrderBy(l => l.Midpoint()).ToList();
        }

        /// <summary>
        /// Returns a List of Lines forming the Polygon's skeleton sorted by the ascending midpoint of each Line.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns>A List of Lines</Line></returns>
        public static List<Line> Skeleton(this Polygon polygon)
        {
            var vertices2d = new List<Vector2d>();
            foreach (var vertex in polygon.Vertices)
            {
                vertices2d.Add(new Vector2d(vertex.X, vertex.Y));
            }
            var skeleton = SkeletonBuilder.Build(vertices2d);
            var lines = new List<Line>();
            foreach (var edgeResult in skeleton.Edges)
            {
                var vertices = new List<Vector3>();
                foreach (var vertex in edgeResult.Polygon)
                {
                    vertices.Add(new Vector3(vertex.X, vertex.Y, 0.0));
                }
                var poly = new Polygon(vertices);
                var segments = (poly.Segments());
                foreach (var segment in segments)
                {
                    if ((polygon.Contains(segment.Start) || polygon.Contains(segment.End)) &&
                        !segment.IsListed(lines))
                    {
                        lines.Add(segment);
                    }
                }
            }
            return lines.OrderBy(l => l.Midpoint()).ToList();
        }


        /// <summary>
        /// Returns a List of Lines forming the Polygon's centerline sorted by the ascending midpoint of each Line.
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns>A List of Lines</Line></returns>
        public static List<Line> Spine(this Polygon polygon)
        {
            var skeleton = polygon.Skeleton();
            var lines = new List<Line>();
            foreach (var line in skeleton)
            {
                if(polygon.Contains(line.Start) && polygon.Contains(line.End))
                {
                    lines.Add(line);
                }
            }
            return lines.OrderBy(l => l.Midpoint()).ToList();
        }
    }
}
