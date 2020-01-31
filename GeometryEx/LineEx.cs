using System;
using System.Linq;
using System.Collections.Generic;
using ClipperLib;
using Elements.Geometry;

namespace GeometryEx
{
    public static class LineEx
    {
        /// <summary>
        /// Constructs the geometric difference between this Line and the supplied Polygons.
        /// </summary>
        /// <param name="difPolys">The list of intersecting Polygons.</param>
        /// <returns>
        /// Returns the longest Line representing the subtraction of the Line intersecting the supplied list of Polygons.
        /// </returns>
        public static Line Difference(this Line line, IList<Polygon> difPolys)
        {
            var thisPath = ToClipperPath(line);
            var polyPaths = new List<List<IntPoint>>();
            foreach (Polygon poly in difPolys)
            {
                polyPaths.Add(poly.ToClipperPath());
            }
            Clipper clipper = new Clipper();
            clipper.AddPath(thisPath, PolyType.ptSubject, false);
            clipper.AddPaths(polyPaths, PolyType.ptClip, true);
            var solution = new PolyTree();
            clipper.Execute(ClipType.ctDifference, solution);
            var soLines = Clipper.OpenPathsFromPolyTree(solution);
            if (soLines.Count == 0)
            {
                return null;
            }
            var lines = new List<Line>();
            foreach (List<IntPoint> path in soLines)
            {
                lines.Add(ToLine(path.Distinct().ToList()));
            }
            return lines.OrderByDescending(l => l.Length()).ToList().First();

        /// <summary>
        /// Constructs the geometric difference between this Line and the supplied Polygons.
        /// </summary>
        /// <param name="difPolys">The list of intersecting Polygons.</param>
        /// <returns>
        /// Returns a list of Lines representing the subtraction of the Lines intersecting the supplied list of Polygons.
        /// </returns>
        public static List<Line> Differences(this Line line, IList<Polygon> difPolys)
        {
            var thisPath = ToClipperPath(line);
            var polyPaths = new List<List<IntPoint>>();
            foreach (Polygon poly in difPolys)
            {
                polyPaths.Add(poly.ToClipperPath());
            }
            Clipper clipper = new Clipper();
            clipper.AddPath(thisPath, PolyType.ptSubject, false);
            clipper.AddPaths(polyPaths, PolyType.ptClip, true);
            var solution = new PolyTree();
            clipper.Execute(ClipType.ctDifference, solution);
            var soLines = Clipper.OpenPathsFromPolyTree(solution);
            var lines = new List<Line>();
            foreach (List<IntPoint> path in soLines)
            {
                lines.Add(ToLine(path.Distinct().ToList()));
            }
            return lines;
        }

        /// <summary>
        /// Creates a collection of Vector3 points representing the division of the linear geometry into the supplied number of segments.
        /// </summary>
        /// <param name="segments">The quantity of desired segments.</param>
        /// <returns>
        /// A List of Vector3 points including the start and end points of the series.
        /// </returns>
        public static List<Vector3> Divide(this Line line, int segments)
        {
            var lines = line.DivideByCount(segments);
            var points = new List<Vector3>();
            foreach (var segment in lines)
            {
                points.Add(segment.Start);
            }
            points.Add(lines.Last().End);
            return points;
        }

        /// <summary>
        /// Extends this Line from its start point by the supplied distance.
        /// </summary>
        /// <param name="length">length by which to extend this line.</param>
        /// <returns>
        /// A new Line.
        /// </returns>
        public static Line ExtendStart(this Line line, double length)
        {
            length += line.Length();
            double extend = Math.Atan2(line.Start.Y - line.End.Y, line.Start.X - line.End.X);
            var startX = line.End.X + length * Math.Cos(extend);
            var startY = line.End.Y + length * Math.Sin(extend);
            return new Line(line.End, new Vector3(startX, startY));
        }

        /// <summary>
        /// Extends this Line from its end point by the supplied distance.
        /// </summary>
        /// <param name="length">length by which to extend this line.</param>
        /// <returns>
        /// A new Line.
        /// </returns>
        public static Line ExtendEnd(this Line line, double length)
        {
            length += line.Length();
            double extend = Math.Atan2(line.End.Y - line.Start.Y, line.End.X - line.Start.X);
            var endX = line.Start.X + length * Math.Cos(extend);
            var endY = line.Start.Y + length * Math.Sin(extend);
            return new Line(line.Start, new Vector3(endX, endY));
        }

        /// <summary>
        /// Finds the implied intersection of this line with a supplied line.
        /// </summary>
        /// <param name="intr">Line to find intersection with this Line.</param>
        /// <returns>
        /// A Vector3 point or null if the lines are parallel.
        /// </returns>
        public static Vector3 Intersection(this Line line, Line intr)
        {
            var lineSlope = (line.End.Y - line.Start.Y) / (line.End.X - line.Start.X);
            var intrSlope = (intr.End.Y - intr.Start.Y) / (intr.End.X - intr.Start.X);
            if (lineSlope.NearEqual(intrSlope) || 
               (Math.Abs(lineSlope) == double.PositiveInfinity && Math.Abs(intrSlope) == double.PositiveInfinity))
            {
                return new Vector3(double.NaN, double.NaN, double.NaN);
            }
            if (Math.Abs(lineSlope) == double.PositiveInfinity && intrSlope.NearEqual(0.0))
            {
                return new Vector3(line.Start.X, intr.Start.Y);
            }
            if (Math.Abs(intrSlope) == double.PositiveInfinity && lineSlope.NearEqual(0.0))
            {
                return new Vector3(intr.Start.X, line.Start.Y);
            }
            double lineB;
            double intrB;
            if (Math.Abs(lineSlope) == double.PositiveInfinity)
            {
                intrB = intr.End.Y - (intrSlope * intr.End.X);
                return new Vector3(line.End.X, intrSlope * line.End.X + intrB);
            }
            if (Math.Abs(intrSlope) == double.PositiveInfinity)
            {
                lineB = line.End.Y - (lineSlope * line.End.X);
                return new Vector3(intr.End.X, lineSlope * intr.End.X + lineB);
            }
            lineB = line.End.Y - (lineSlope * line.End.X);
            intrB = intr.End.Y - (intrSlope * intr.End.X);
            var x = (intrB - lineB) / (lineSlope - intrSlope);
            var y = lineSlope * x + lineB;
            return new Vector3(x, y);
        }

        /// <summary>
        /// Returns whether this line shares a point and a slope with the supplied line.
        /// </summary>
        /// <param name="thatLine">Line to compare to this line.</param>
        /// <returns>
        /// True if the lines share a point and have the same slope.
        /// </returns>
        public static bool IsContiguousWith(this Line line, Line thatLine)
        {
            var lineSlope = (line.End.Y - line.Start.Y) / (line.End.X - line.Start.X);
            var thatSlope = (thatLine.End.Y - thatLine.Start.Y) / (thatLine.End.X - thatLine.Start.X);
            if (lineSlope == double.NegativeInfinity)
            {
                lineSlope = double.PositiveInfinity;
            }
            if (thatSlope == double.NegativeInfinity)
            {
                thatSlope = double.PositiveInfinity;
            }
            if (!lineSlope.NearEqual(thatSlope))
            {
                return false;
            }
            if (line.End.IsAlmostEqualTo(thatLine.End) || line.Start.IsAlmostEqualTo(thatLine.Start) ||
                line.Start.IsAlmostEqualTo(thatLine.End) || line.End.IsAlmostEqualTo(thatLine.Start))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether a line is parallel to the x-axis.
        /// </summary>
        /// <returns>
        /// True if the line's slope is zero.
        /// </returns>
        public static bool IsHorizontal(this Line line)
        {
            var slope = (line.End.Y - line.Start.Y) / (line.End.X - line.Start.X);
            if (slope.NearEqual(0.0))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns whether this line is parallel to the supplied line.
        /// </summary>
        /// <param name="thatLine">Line to compare to this line.</param>
        /// <returns>
        /// True if the lines have equal slopes.
        /// </returns>
        public static bool IsParallelTo(this Line line, Line thatLine)
        {
            var lineSlope = (line.End.Y - line.Start.Y) / (line.End.X - line.Start.X);
            var thatSlope = (thatLine.End.Y - thatLine.Start.Y) / (thatLine.End.X - thatLine.Start.X);
            if (lineSlope == double.NegativeInfinity)
            {
                lineSlope = double.PositiveInfinity;
            }
            if (thatSlope == double.NegativeInfinity)
            {
                thatSlope = double.PositiveInfinity;
            }
            if (lineSlope.NearEqual(thatSlope))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns whether this line is perpendicular to the supplied line.
        /// </summary>
        /// <param name="thatLine">Line to compare to this line.</param>
        /// <returns>
        /// True if the product of the slopes is -1.
        /// </returns>
        public static bool IsPerpendicularTo(this Line line, Line thatLine)
        {
            var lineSlope = (line.End.Y - line.Start.Y) / (line.End.X - line.Start.X);
            var thatSlope = (thatLine.End.Y - thatLine.Start.Y) / (thatLine.End.X - thatLine.Start.X);
            if (((lineSlope == double.PositiveInfinity || lineSlope == double.NegativeInfinity) && thatSlope == 0.0) ||
                ((thatSlope == double.PositiveInfinity || thatSlope == double.NegativeInfinity) && lineSlope == 0.0))
            {
                return true;
            }
            if (lineSlope * thatSlope == -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns whether a line is parallel to the y-axis.
        /// </summary>
        /// <returns>
        /// True if the line's slope resolves to an infinite value.
        /// </returns>
        public static bool IsVertical (this Line line)
        {
            var slope = (line.End.Y - line.Start.Y) / (line.End.X - line.Start.X);
            if (slope == double.PositiveInfinity || slope == double.NegativeInfinity)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates a new line from this Line and a supplied line with a single coincident endpoint and identical slope.
        /// </summary>
        /// <returns>
        /// A new Line.
        /// </returns>
        public static Line JoinTo (this Line line, Line join)
        {
            if (!line.Slope().NearEqual(join.Slope()) ||
               (!line.Start.IsAlmostEqualTo(join.Start) &&
                !line.Start.IsAlmostEqualTo(join.End) &&
                !line.End.IsAlmostEqualTo(join.Start) &&
                !line.End.IsAlmostEqualTo(join.End)))
            {
                return null;
            }
            var length = line.Length() + join.Length();
            if (line.Start.DistanceTo(join.Start).NearEqual(length))
            {
                return new Line(line.Start, join.Start);
            }
            if (line.Start.DistanceTo(join.End).NearEqual(length))
            {
                return new Line(line.Start, join.End);
            }
            if (line.End.DistanceTo(join.Start).NearEqual(length))
            {
                return new Line(line.End, join.Start);
            }
            if (line.End.DistanceTo(join.End).NearEqual(length))
            {
                return new Line(line.End, join.End);
            }
            return null;
        }

        /// <summary>
        /// Returns the midpoint between the line's start and end.
        /// </summary>
        /// <returns>
        /// A Vector3 point.
        /// </returns>
        public static Vector3 Midpoint(this Line line)
        {
            return new Vector3((line.Start.X + line.End.X) * 0.5, (line.Start.Y + line.End.Y) * 0.5);
        }

        /// <summary>
        /// Returns a new line displaced from the supplied line along a 2D vector calculated between the supplied Vector3 points.
        /// </summary>
        /// <param name="line">The Line instance to be copied.</param>
        /// <param name="from">The Vector3 base point of the move.</param>
        /// <param name="to">The Vector3 target point of the move.</param>
        /// <returns>
        /// A new Line.
        /// </returns>
        public static Line MoveFromTo(this Line line, Vector3 from, Vector3 to)
        {
            var v = new Vector3(to.X - from.X, to.Y - from.Y);
            return new Line(line.Start + v, line.End + v);
        }

        /// <summary>
        /// Returns a point the supplied distance along the line.
        /// </summary>
        /// <param name="distance">Distance along the line to the desired point.</param>
        /// <returns>
        /// A Vector3 point on the line.
        /// If the distance exceed the length of the line, returns the end point of the line.
        /// </returns>
        public static Vector3 PositionAt(this Line line, double distance)
        {
            if (distance >= line.Length())
            {
                return line.End;
            }
            return line.PointAt(distance / line.Length());
        }

        /// <summary>
        /// Creates a new line from the supplied line rotated around the supplied pivot point by the specified angle in degrees.
        /// </summary>
        /// <param name="line">Line instance to be copied and rotated.</param>
        /// <param name="pivot">Vector3 base point of the rotation.</param>
        /// <param name="angle">Desired rotation angle in degrees.</param>
        /// <returns>
        /// A new Line.
        /// </returns>
        public static Line Rotate(this Line line, Vector3 pivot, double angle)
        {
            var theta = angle * (Math.PI / 180);
            var sX = (Math.Cos(theta) * (line.Start.X - pivot.X)) - (Math.Sin(theta) * (line.Start.Y - pivot.Y)) + pivot.X;
            var sY = (Math.Sin(theta) * (line.Start.X - pivot.X)) + (Math.Cos(theta) * (line.Start.Y - pivot.Y)) + pivot.Y;
            var eX = (Math.Cos(theta) * (line.End.X - pivot.X)) - (Math.Sin(theta) * (line.End.Y - pivot.Y)) + pivot.X;
            var eY = (Math.Sin(theta) * (line.End.X - pivot.X)) + (Math.Cos(theta) * (line.End.Y - pivot.Y)) + pivot.Y;
            return new Line(new Vector3(sX, sY), new Vector3(eX, eY));
        }

        /// <summary>
        /// Returns a list of lines by dividing the supplied line by the supplied length from the line's start point.
        /// </summary>
        /// <param name="length">Longest allowable segment.</param>
        /// <returns>
        /// A list of lines of the specified length and a shorter line representing any remainder, or a list containing a copy of the supplied line if the supplied length is greater than the line.
        /// </returns>
        public static List<Line> Segment(this Line line, double length, double minimum = 1e-09)
        {
            var lines = new List<Line>();
            var segments = line.Length() / length;
            if (segments <= 1.0 || minimum > length)
            {
                lines.Add(line);
                return lines;
            }
            var start = line.Start;
            for (int i = 0; i < Math.Floor(segments); i++)
            {
                var end = line.PointAt(length / line.Length() * (i + 1));
                if (start.IsAlmostEqualTo(end))
                {
                    break;
                }
                var newLine = new Line(start, end);
                lines.Add(newLine);
                start = newLine.End;
            }
            start = lines.Last().End;
            if (!start.IsAlmostEqualTo(line.End))
            {
                var remainder = new Line(start, line.End);
                if (remainder.Length() < minimum)
                {
                    var joiner = lines.Last();
                    lines.Reverse();
                    lines = lines.Skip(1).ToList();
                    lines.Reverse();
                    var tstJoin = joiner.JoinTo(remainder);
                    if (tstJoin != null)
                    {
                        remainder = new Line(tstJoin.Start, tstJoin.End);
                    }
                }
                lines.Add(remainder);
            }
            return lines;
        }

        /// <summary>
        /// Returns a list of lines by dividing the supplied line by the supplied length from the specified start point.
        /// </summary>
        /// <param name="length">Longest allowable segment.</param>
        /// <returns>
        /// A list of lines of the specified length and shorter line or lines representing any remainder, or a list containing a copy of the supplied line if the supplied length is greater than the line.
        /// </returns>
        public static List<Line> SegmentFrom(this Line line, double length, DivideFrom from = DivideFrom.Start, double minimum = 1e-09)
        {
            var lines = new List<Line>();
            if (length >= line.Length())
            {
                lines.Add(line);
                return lines;
            }
            switch (from)
            {
                case DivideFrom.Center:
                    lines.AddRange(new Line(line.Midpoint(), line.Start).Segment(length, minimum));
                    lines.AddRange(new Line(line.Midpoint(), line.End).Segment(length, minimum));
                    break;
                case DivideFrom.Centered:
                    var start = line.PositionAt((line.Length() * 0.5) - (length * 0.5));
                    var end = line.PositionAt((line.Length() * 0.5) + (length * 0.5));
                    lines.Add(new Line(start, end));
                    lines.AddRange(new Line(start, line.Start).Segment(length, minimum));
                    lines.AddRange(new Line(end, line.End).Segment(length, minimum));
                    break;
                case DivideFrom.End:
                    lines.AddRange(new Line(line.End, line.Start).Segment(length, minimum));
                    break;
                default:
                    lines.AddRange(line.Segment(length, minimum));
                    break;
            }
            return lines;
        }

        /// <summary>
        /// Returns the slope of the line, normalizing a vertical line to a slope of positive infinity.
        /// </summary>
        /// <returns>
        /// A double representing the slope of the line.
        /// </returns>
        public static double Slope (this Line line)
        {
            var slope = (line.End.Y - line.Start.Y) / (line.End.X - line.Start.X);
            if (slope == double.NegativeInfinity)
            {
                slope = double.PositiveInfinity;
            }
            return slope;
        }

        private const double scale = 1024.0;

        /// <summary>
        /// Construct a clipper path from a Polygon.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static List<IntPoint> ToClipperPath(this Line line)
        {
            var path = new List<IntPoint>
            {
                new IntPoint(line.Start.X * scale, line.Start.Y * scale),
                new IntPoint(line.End.X * scale, line.End.Y * scale)
            };
            return path.ToList();
        }

        /// <summary>
        /// Construct a Line from a clipper path 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        internal static Line ToLine(this List<IntPoint> line)
        {
            return new Line(new Vector3(line.First().X / scale, line.First().Y / scale),
                            new Vector3(line.Last().X / scale, line.Last().Y / scale));
        }
    }
}
