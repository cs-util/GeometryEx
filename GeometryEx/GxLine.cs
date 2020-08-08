using System;
using System.Collections.Generic;
using Elements.Geometry;

namespace GeometryEx
{
    /// <summary>
    /// A line with editable endpoints.
    /// </summary>
    public class GxLine
    {
        public GxLine()
        {
            s = Vector3.Origin;
            e = Vector3.XAxis;
        }

        public GxLine(Vector3 end)
        {
            if (end.IsAlmostEqualTo(Vector3.Origin))
            {
                return;
            }
            s = Vector3.Origin;
            e = end;
        }

        public GxLine(Vector3 start, Vector3 end)
        {
            if (start.IsAlmostEqualTo(End))
            {
                return;
            }
            s = start;
            e = end;
        }

        public GxLine(Line line)
        {
            s = line.Start;
            e = line.End;
        }

        public Line ToLine()
        {
            return new Line(Start, End);
        }

        private Vector3 s;
        private Vector3 e;
        public Vector3 Start
        {
            get
            {
                return s;
            }
            set
            {
                s = !value.IsAlmostEqualTo(e) ? value : s;
            }
        }
        public Vector3 End
        {
            get
            {
                return e;
            }
            set
            {
                e = !value.IsAlmostEqualTo(s) ? value : e;
            }
        }
        
        public double Length
        {
            get
            {
                return s.DistanceTo(e);
            }
        }

        /// <summary>
        /// Finds the implied intersection of this GxLine with a supplied GxLine.
        /// </summary>
        /// <param name="intr">GxLine to find intersection with this GxLine.</param>
        /// <returns>
        /// A Vector3 point or null if the lines are parallel.
        /// </returns>
        public Vector3 Intersection(GxLine line)
        {
            return ToLine().Intersection(line.ToLine());
        }

        /// <summary>
        /// Returns true if this GxLine is parallel to the supplied GxLine.
        /// </summary>
        /// <param name="thatLine">Line to compare to this line.</param>
        /// <returns>
        /// True if the Lines have equal slopes.
        /// </returns>
        public bool IsParallelTo(GxLine line)
        {
            return ToLine().IsParallelTo(line.ToLine());
        }

        /// <summary>
        /// Returns the perpendicular distance from this GxLine to the supplied Vector3 point.
        /// </summary>
        /// <param name="point">Vector3 representing a point.</param>
        /// <returns>A double.</returns>
        public double PerpendicularDistanceTo(Vector3 point)
        {
            return ToLine().PerpendicularDistanceTo(point);
        }
    }
}
