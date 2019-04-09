using System;
using System.Collections.Generic;
using Elements.Geometry;

namespace GeometryEx
{
    public static class LineEx
    {
        /// <summary>
        /// Creates a collection of Vector3 points representing the division of the linear geometry into the supplied number of segments.
        /// </summary>
        /// <param name="segments">The quantity of desired segments.</param>
        /// <returns>
        /// A List of Vector3 points.
        /// </returns>
        public static List<Vector3> Divide(this Line line, int segments)
        {
            var pointList = new List<Vector3>()
            {
                line.Start
            };
            var percent = 1.0 / segments;
            var factor = 1;
            var at = percent * factor;
            for (int i = 0; i < segments; i++)
            {
                pointList.Add(line.PointAt(at));
                at = percent * ++factor;
            }
            return pointList;
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
            if (lineSlope.NearEqual(intrSlope))
            {
                return null;
            }
            if ((lineSlope == double.NegativeInfinity || lineSlope == double.PositiveInfinity) 
                && intrSlope.NearEqual(0.0))
            {
                return new Vector3(line.Start.X, intr.Start.Y);
            }
            if ((intrSlope == double.NegativeInfinity || intrSlope == double.PositiveInfinity) 
                && lineSlope.NearEqual(0.0))
            {
                return new Vector3(intr.Start.X, line.Start.Y);
            }
            double lineB;
            double intrB;
            if (lineSlope == double.NegativeInfinity || lineSlope == double.PositiveInfinity)
            {
                intrB = intr.End.Y - (intrSlope * intr.End.X);
                return new Vector3(line.End.X, intrSlope * line.End.X + intrB);
            }
            if (intrSlope == double.NegativeInfinity || intrSlope == double.PositiveInfinity)
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
        public static List<Line> Segment(this Line line, double max, double min)
        {
            var lines = new List<Line>();
            var segments = line.Length() / max;
            if (segments <= 1.0 || min > max)
            {
                lines.Add(line);
                return lines;
            }
            var start = line.Start;
            for (int i = 0; i < Math.Floor(segments); i++)
            {
                var end = line.PointAt(max / line.Length() * (i + 1));
                if (start.IsAlmostEqualTo(end))
                {
                    break;
                }
                var newLine = new Line(start, end);
                lines.Add(newLine);
                start = newLine.End;
            }
            if (lines.Count > 0)
            {
                start = lines[lines.Count - 1].End;
                if (!start.IsAlmostEqualTo(line.End))
                {
                    var remainder = new Line(start, line.End);
                    if (remainder.Length() < min)
                    {
                        remainder = new Line(lines[lines.Count - 1].Start, remainder.End);
                        lines.RemoveAt(lines.Count - 1);
                    }
                    lines.Add(remainder);
                }
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
        public static List<Line> SegmentFrom(this Line line, double max, double min, DivideFrom from = DivideFrom.Start)
        {
            var lines = new List<Line>();
            if (max >= line.Length())
            {
                lines.Add(line);
                return lines;
            }
            switch (from)
            {
                case DivideFrom.Center:
                    lines.AddRange(new Line(line.Midpoint(), line.Start).Segment(max, min));
                    lines.AddRange(new Line(line.Midpoint(), line.End).Segment(max, min));
                    break;
                case DivideFrom.Centered:
                    var start = line.PositionAt((line.Length() * 0.5) - (max * 0.5));
                    var end = line.PositionAt((line.Length() * 0.5) + (max * 0.5));
                    lines.Add(new Line(start, end));
                    lines.AddRange(new Line(start, line.Start).Segment(max, min));
                    lines.AddRange(new Line(end, line.End).Segment(max, min));
                    break;
                case DivideFrom.End:
                    lines.AddRange(new Line(line.End, line.Start).Segment(max, min));
                    break;
                default:
                    lines.AddRange(line.Segment(max, min));
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
    }
}
