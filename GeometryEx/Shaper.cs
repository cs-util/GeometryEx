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
        /// <param name="area">Desired area of the new Polygon.</param>
        /// <param name="orient">Relative cardinal direction in which the new Polygon will be placed.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon AdjacentArea(Polygon polygon, double area, Orient orient)
        {
            if (polygon == null)
            {
                return null;
            }
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
            if (polygon == null)
            {
                return null;
            }
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
        /// Creates a convex hull Polygon from the vertices of all supplied Polygons.
        /// </summary>
        /// <param name="polygons">A list of Polygons from which to extract vertices.</param>
        /// <returns>A new Polygon.</returns>
        public static Polygon ConvexHullFromPolygons(List<Polygon> polygons)
        {
            var points = new List<Vector3>();
            foreach (var polygon in polygons)
            {
                points.AddRange(polygon.Vertices);
            }
            return new Polygon(ConvexHull.MakeHull(points));
        }

        /// <summary>
        /// Creates two Lines intersecting at their implied intersection, or the original Lines if the Lines are parallel
        /// </summary>
        /// <param name="thisLine"></param>
        /// <param name="thatLine"></param>
        /// <returns></returns>
        public static List<Line> CornerLines (Line thisLine, Line thatLine)
        {
            if (thisLine.IsParallelTo(thatLine))
            {
                return 
                    new List<Line>
                    {
                        thisLine,
                        thatLine
                    };
            }
            var inters = thisLine.Intersection(thatLine);
            var thisGxLine = new GxLine(thisLine);
            var thatGxLine = new GxLine(thatLine);
            if (inters.DistanceTo(thisGxLine.Start) <= inters.DistanceTo(thisGxLine.End))
            {
                thisGxLine.Start = inters;
            }
            else
            {
                thisGxLine.End = inters;
            }
            if (inters.DistanceTo(thatGxLine.Start) <= inters.DistanceTo(thatGxLine.End))
            {
                thatGxLine.Start = inters;
            }
            else
            {
                thatGxLine.End = inters;
            }
            return
                new List<Line>
                {
                    thisGxLine.ToLine(),
                    thatGxLine.ToLine()
                };
        }

        /// <summary>
        /// Constructs the geometric differences between two supplied Lists of Polygons.
        /// </summary>
        /// <param name="polygons">List of subject Polygons for the difference calculation.</param>
        /// <param name="diffs">List of clipping Polygons for the difference calculation.</param>
        /// <returns>
        /// A List of Polygons representing the subtraction of the clipping Polygons from the subject Polygons.
        /// </returns>
        public static List<Polygon> Differences(List<Polygon> polygons, List<Polygon> diffs, double tolerance = 0.01)
        {
            diffs = Merge(diffs);
            var differs = new List<Polygon>();
            var clipper = new Clipper();
            foreach (var polygon in polygons)
            {
                clipper.AddPath(polygon.PolygonToClipper(), PolyType.ptSubject, true);
            }
            foreach (var differ in diffs)
            {
                clipper.AddPath(differ.PolygonToClipper(), PolyType.ptClip, true);
            }
            var solution = new List<List<IntPoint>>();
            clipper.Execute(ClipType.ctDifference, solution, PolyFillType.pftNonZero);
            if (solution.Count == 0)
            {
                return differs;
            }
            foreach (var path in solution)
            {
                var diff = PolygonFromClipper(path);
                if (diff == null)
                {
                    continue;
                }
                if (diff.IsClockWise())
                {
                    diff = diff.Reversed();
                }
                differs.Add(diff);
            }
            var polys = Merge(differs).OrderByDescending(p => Math.Abs(p.Area())).ToList();
            differs.Clear();
            foreach (var poly in polys)
            {
                if (poly.Area() < tolerance)
                {
                    break;
                }
                differs.Add(poly);
            }
            return differs;
        }

        /// <summary>
        /// Creates a list of Polygons fitted within a supplied intersecting perimeter.
        /// </summary>
        /// <param name="polygon">A Polygon to intersect with the supplied within perimeter Polygon.</param>
        /// <param name="within">Polygon acting as a constraining outer boundary.</param>
        /// <returns>
        /// A List of Polygons.
        /// </returns>
        public static List<Polygon> FitWithin(Polygon polygon, Polygon within)
        {
            var intersects = new List<Polygon>();
            if (!within.Intersects(polygon))
            {
                return intersects;
            }
            intersects = within.Intersection(polygon).ToList();
            if (intersects.Count == 0)
            {
                return intersects;
            }
            var polygons = new List<Polygon>();
            foreach (var intersect in intersects)
            {
                if (intersect.IsClockWise())
                {
                    polygons.Add(intersect.Reversed());
                    continue;
                }
                polygons.Add(intersect);
            }
            return polygons;
        }

        /// <summary>
        /// Returns the List of Polygons wholly contained within the specified coordinate system quadrant.
        /// </summary>
        /// <param name="polygons">A List of Polygons to test for positioning.</param>
        /// <param name="quad">The Quadrant to test the supplied Polygon for inclusion.</param>
        /// <returns>A List of Polygons.</returns>
        public static List<Polygon> InQuadrant(List<Polygon> polygons, Quadrant quad)
        {
            var quadPolygons = new List<Polygon>();
            foreach (var polygon in polygons)
            {
                var inQuad = true;
                foreach (var vertex in polygon.Vertices)
                {
                    if (quad == Quadrant.I && (vertex.X < 0.0 || vertex.Y < 0.0))
                    {
                        inQuad = false;
                        break;
                    }
                    if (quad == Quadrant.II && (vertex.X > 0.0 || vertex.Y < 0.0))
                    {
                        inQuad = false;
                        break;
                    }
                    if (quad == Quadrant.III && (vertex.X > 0.0 || vertex.Y > 0.0))
                    {
                        inQuad = false;
                        break;
                    }
                    if (quad == Quadrant.IV && (vertex.X < 0.0 || vertex.Y > 0.0))
                    {
                        inQuad = false;
                        break;
                    }
                }
                if (inQuad)
                {
                    quadPolygons.Add(polygon);
                }
            }
            return quadPolygons;
        }

        /// <summary>
        /// Constructs the geometric intersections between two supplied Lists of Polygons.
        /// </summary>
        /// <param name="polygons">List of subject Polygons for the difference calculation.</param>
        /// <param name="inters">List of clipping Polygons for the difference calculation.</param>
        /// <returns>
        /// A List of Polygons representing the subtraction of the clipping Polygons from the subject Polygons.
        /// </returns>
        public static List<Polygon> Intersections(List<Polygon> polygons, List<Polygon> inters, double tolerance = 0.01)
        {
            inters = Merge(inters);
            var differs = new List<Polygon>();
            var clipper = new Clipper();
            foreach (var polygon in polygons)
            {
                clipper.AddPath(polygon.PolygonToClipper(), PolyType.ptSubject, true);
            }
            foreach (var differ in inters)
            {
                clipper.AddPath(differ.PolygonToClipper(), PolyType.ptClip, true);
            }
            var solution = new List<List<IntPoint>>();
            clipper.Execute(ClipType.ctIntersection, solution, PolyFillType.pftNonZero);
            if (solution.Count == 0) return differs;
            foreach (var path in solution)
            {
                var diff = PolygonFromClipper(path);
                if (diff == null) continue;
                if (diff.IsClockWise()) diff = diff.Reversed();
                differs.Add(diff);
            }
            var polys = Merge(differs).OrderByDescending(p => Math.Abs(p.Area())).ToList();
            differs.Clear();
            foreach (var poly in polys)
            {
                if (poly.Area() < tolerance) break;
                differs.Add(poly);
            }
            return differs;
        }

        /// <summary>
        /// Constructs a List of Lines in order from pairs in a List of Vector3 points.
        /// </summary>
        /// <param name="points">List of vertices to convert to Lines.</param>
        /// <param name="close">If true also creates a Line from the Last to First points of the List.</param>
        /// <returns>List of Lines.</returns>
        public static List<Line> LinesFromPoints(List<Vector3> points, bool close = false)
        {
            var lines = new List<Line>();
            if (points.Count == 0)
            {
                return lines;
            }
            for (var i = 0; i < points.Count - 1; i++)
            {
                lines.Add(new Line(points[i], points[i + 1]));
            }
            if (close)
            {
                lines.Add(new Line(points.Last(), points.First()));
            }
            return lines;
        }

        /// <summary>
        /// Constructs the geometric union of the supplied list of Polygons.
        /// </summary>
        /// <param name="polygons">The list of Polygons to be combined.</param>
        /// <returns>
        /// A List of Polygons.
        /// </returns>
        public enum FillType { EvenOdd, NonZero, Positive, Negative };
        public static List<Polygon> Merge(List<Polygon> polygons,                                           
                                          FillType fillType = FillType.NonZero, 
                                          double tolerance = 0.0)
        {
            var resultPolygons = new List<Polygon>();
            if (polygons == null || polygons.Count == 0)
            {
                return resultPolygons;
            }
            var filtyp = (PolyFillType)fillType;
            var polyPaths = new List<List<IntPoint>>();
            foreach (Polygon polygon in polygons)
            {
                polyPaths.Add(polygon.PolygonToClipper());
            }
            Clipper clipper = new Clipper();
            clipper.AddPaths(polyPaths, PolyType.ptClip, true);
            clipper.AddPaths(polyPaths, PolyType.ptSubject, true);
            var solution = new List<List<IntPoint>>();
            clipper.Execute(ClipType.ctUnion, solution, filtyp);
            if (solution.Count == 0)
            {
                return resultPolygons;
            }
            foreach (var solved in solution)
            {
                var polygon = solved.ToList().PolygonFromClipper();
                if (polygon == null)
                {
                    continue;
                }
                if (polygon.IsClockWise())
                {
                    polygon = polygon.Reversed();
                }
                resultPolygons.Add(polygon);
            }
            tolerance = Math.Abs(tolerance);
            if (tolerance.NearEqual(0.0))
            {
                return resultPolygons;
            }
            var mergePolygons = new List<Polygon>();
            foreach(var polygon in resultPolygons)
            {
                mergePolygons.Add(polygon.Simplify(tolerance));
            }
            return mergePolygons;
        }

        /// <summary>
        /// Constructs the set of nearby Polygons from 8 bounding boxes as delivered by the nearPolygon and its orthogonal at each vertex of the Polygon.
        /// </summary>
        /// <param name="polygon">The static Polygon serving as the vertex positioning.</param>
        /// <param name="nearPolygon">The Polygon to array around each Polygon vertex.</param>
        /// <param name="rotated">Whether rotated Polygons should be returned.</param>
        /// <returns></returns>
        public static List<Polygon> NearPolygons(Polygon polygon, Polygon nearPolygon, bool rotated = false) 
        {
            var polygons = new List<Polygon>();
            var points = polygon.Vertices;
            var compass = nearPolygon.Compass();
            foreach (var point in points)
            {
                polygons.Add(nearPolygon.MoveFromTo(compass.SW, point));
                polygons.Add(nearPolygon.MoveFromTo(compass.SE, point));
                polygons.Add(nearPolygon.MoveFromTo(compass.NE, point));
                polygons.Add(nearPolygon.MoveFromTo(compass.NW, point));
            }
            if (!rotated)
            {
                return polygons;
            }
            compass = nearPolygon.Rotate(Vector3.Origin, 90.0).Compass();
            foreach (var point in points)
            {
                polygons.Add(nearPolygon.MoveFromTo(compass.SW, point));
                polygons.Add(nearPolygon.MoveFromTo(compass.SE, point));
                polygons.Add(nearPolygon.MoveFromTo(compass.NE, point));
                polygons.Add(nearPolygon.MoveFromTo(compass.NW, point));
            }
            return polygons;
        }

        /// <summary>
        /// Returns the List of Polygons that do not intersect the supplied polygon.
        /// </summary>
        /// <param name="polygons">A List of Polygons to test against.</param>
        /// <param name="polygons">A List of Polygons to test for intersection.</param>
        /// <returns>A List of non-intersecting Polygons.</returns>
        public static List<Polygon> NonIntersecting(List<Polygon> placed, List<Polygon> polygons)
        {
            var nonPolygons = new List<Polygon>();
            foreach (var polygon in polygons)
            {
                if (polygon.Intersects(placed))
                {
                    continue;
                }
                nonPolygons.Add(polygon);
            }
            return nonPolygons;
        }

        /// <summary>
        /// Returns a Polygon positioned diagonally adjacent to another Polygon.
        /// </summary>
        /// <param name="polygon">The static Polygon.</param>
        /// <param name="place">The Polygon to be placed adjacent to the static Polygon.</param>
        /// <param name="corner">The corner at which to place the second Polygon as determined by the Polygon bounding boxes.</param>
        /// <returns>A new Polygon placed diagonally adjacent to the static Polygon.</returns>
        public static Polygon PlaceDiagonal(Polygon polygon, Polygon place, Corner corner)
        {
            var comPly = polygon.Compass();
            var comPlc = place.Compass();
            switch (corner)
            {
                case Corner.NW:
                    place = place.MoveFromTo(comPlc.SE, comPly.NW);
                    break;
                case Corner.NE:
                    place = place.MoveFromTo(comPlc.SW, comPly.NE);
                    break;
                case Corner.SW:
                    place = place.MoveFromTo(comPlc.NE, comPly.SW);
                    break;
                case Corner.SE:
                    place = place.MoveFromTo(comPlc.NW, comPly.SE);
                    break;
            }
            return place;
        }

        /// <summary>
        /// Returns a Polygon positioned orthogonally adjacent to another Polygon by determining the minimal x or y expansion of the first Polygon with the second Polygon as derived from the bounding boxes of the delivered Polygons. The second Polygon is rotated to achieve a minimal dimensional expansion relative to the bounding box of the first Polygon.
        /// </summary>
        /// <param name="polygon">The static Polygon.</param>
        /// <param name="place">The Polygon to be placed adjacent to the static Polygon.</param>
        /// <param name="northeast">Place the Polygon north or east of the static Polygon, otherwise place it west or south.</param>
        /// <param name="minCoord">If true, minimize x or y coordinate placement.</param>
        /// <returns>A new Polygon rotated and placed adjacent to the static Polygon.</returns>
        public static Polygon PlaceOrthogonal(Polygon polygon, Polygon place, 
                                              bool northeast = true, bool minCoord = true)
        {
            var vrtPly = false;
            var vrtPlc = false;
            var comPly = polygon.Compass();
            var comPlc = place.Compass();
            if (comPly.AspectRatio < 1.0) { vrtPly = true; }
            if (comPlc.AspectRatio < 1.0) { vrtPlc = true; }
            if (vrtPly != vrtPlc) { place = place.Rotate(place.Centroid(), 90.0); }
            comPlc = place.Compass();
            if (northeast && vrtPly)
            {
                if (minCoord) { place = place.MoveFromTo(comPlc.SW, comPly.SE); }
                else { place = place.MoveFromTo(comPlc.NW, comPly.NE); }
            }
            if (northeast && !vrtPly)
            {
                if (minCoord) { place = place.MoveFromTo(comPlc.SW, comPly.NW); }
                else { place = place.MoveFromTo(comPlc.SE, comPly.NE); }
            }
            if (!northeast && vrtPly)
            {
                if (minCoord) { place = place.MoveFromTo(comPlc.SE, comPly.SW); }
                else { place = place.MoveFromTo(comPlc.NE, comPly.NW); }
            }
            if (!northeast && !vrtPly)
            {
                if (minCoord) { place = place.MoveFromTo(comPlc.NW, comPly.SW); }
                else { place = place.MoveFromTo(comPlc.NE, comPly.SE); }
            }
            return place;
        }

        /// <summary>
        /// Creates a rectangular Polygon of the supplied length to width proportion at the supplied area with its southwest corner at the origin.
        /// </summary>
        /// <param name="area">Required area of the Polygon.</param>
        /// <param name="ratio">Ratio of width to depth.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon RectangleByArea(double area = 1.0, double ratio = 1.0)
        {
            area = Math.Abs(area);
            ratio = Math.Abs(ratio);
            if (area.NearEqual(0.0) || ratio.NearEqual(0.0))
            {
                return null;
            }
            var x = Math.Sqrt(area * ratio);
            var y = area / Math.Sqrt(area * ratio);
            return Polygon.Rectangle(Vector3.Origin, new Vector3(x, y));
        }

        /// <summary>
        /// Creates a rectangular Polygon of the supplied length to width proportion with its southwest corner at the origin.
        /// </summary>
        /// <param name="ratio">Ratio of width to depth.</param>
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon RectangleByRatio(double ratio = 1.0)
        {
            ratio = Math.Abs(ratio);
            if (ratio.NearEqual(0.0))
            {
                return null;
            }
            return Polygon.Rectangle(Vector3.Origin, new Vector3(1.0, ratio));
        }

        /// <summary>
        /// Reduces a quantity of points tested against a deviation tolerance.
        /// </summary>
        /// <param name="points">A List of Points to test.</param>
        /// <param name="tolerance">The deviation tolerance for inclusion in the final Vector3 List.</param>
        /// <returns>
        /// A List of Vector3 points.
        /// </returns>
        public static List<Vector3> Simplify(List<Vector3> points, double tolerance)
        {
            if (points.Count == 3)
            {
                return points;
            }
            return SimplifyNet.Simplify(points, tolerance);
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
        public static Polygon C(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width >= size.X || width * 3 >= size.Y)
            {
                return null;
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
        public static Polygon E(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width >= size.X || width * 3 >= size.Y)
            {
                return null;
            }
            var halfWidth = width * 0.5;
            var xAxis = size.Y * 0.5;
            return
                new Polygon
                (
                    new[]
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
        public static Polygon F(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width >= size.X || width * 2 >= size.Y)
            {
                return null;
            }
            var halfWidth = width * 0.5;
            var xAxis = size.Y * 0.5;
            return
                new Polygon
                (
                    new[]
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
        /// <returns>
        /// A new Polygon.
        /// </returns>
        public static Polygon H(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width * 2 >= size.X || width >= size.Y)
            {
                return null;
            }
            var halfWidth = width * 0.5;
            var xAxis = size.Y * 0.5;
            var rightWest = size.X - width;
            return
                new Polygon
                (
                    new[]
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
        public static Polygon L(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width >= size.X || width >= size.Y)
            {
                return null;
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
        public static Polygon T(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width >= size.X || width >= size.Y)
            {
                return null;
            }
            var halfWidth = width * 0.5;
            var yAxis = origin.X + (size.X * 0.5);
            return
                new Polygon
                (
                    new[]
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
        public static Polygon U(Vector3 origin, Vector3 size, double width)
        {
            if (size.X <= 0 || size.Y <= 0 || width * 2 >= size.X || width >= size.Y)
            {
                return null;
            }
            return
                new Polygon
                (
                    new[]
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
        public static Polygon X(Vector3 origin, Vector3 size, double width)
        {
            if (width >= Math.Abs(size.X - origin.X) || width >= Math.Abs(size.Y - origin.Y))
            {
                return null;
            }
            var halfWidth = width * 0.5;
            var xAxis = origin.Y + (size.Y * 0.5);
            var yAxis = origin.X + (size.X * 0.5);
            return
                new Polygon
                (
                    new[]
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
        /// Tests if two doubles are effectively equal within a tolerance.
        /// </summary>
        /// <param name="thisValue">Lower bound of the random range.</param>
        /// <param name="thatValue">Upper bound of the random range.</param>
        /// <param name="tolerace">Tolerance for deviation from mathematical equality.</param>
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
            var scale = 10000.0;
            var rnd = new Random();
            double next = rnd.Next((int)Math.Round(minvalue * scale), (int)Math.Round(maxvalue * scale));
            return next / scale;
        }

        /// <summary>
        /// Check if any of lines have zero length.
        /// </summary>
        public static bool ZeroLength(List<Line> lines)
        {
            if (lines.Count == 0)
            {
                return false;
            }
            foreach (var s in lines)
            {
                if (s.Length().NearEqual(0.0))
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
        public static bool SelfIntersects(List<Line> lines)
        {
            if (lines.Count == 0)
            {
                return true;
            }
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

        public const double SCALE = 1000000000000.0;

        /// <summary>
        /// Construct a clipper path from a Polygon.
        /// </summary>
        /// <param name="scale">Scaling factor for Clipper coordinate translation.</param>
        /// <returns></returns>
        internal static List<IntPoint> PolygonToClipper(this Polygon p, double scale = SCALE)
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
        /// <param name="scale">Scaling factor for Clipper coordinate translation.</param>
        /// <returns></returns>
        internal static Polygon PolygonFromClipper(this List<IntPoint> p, double scale = SCALE)
        {
            var points = p.Select(v => new Vector3(v.X / scale, v.Y / scale)).Distinct().ToList();
            var lines = LinesFromPoints(points);
            if (ZeroLength(lines) || SelfIntersects(lines))
            {
                return null;
            }
            try
            {
                return new Polygon(points);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
