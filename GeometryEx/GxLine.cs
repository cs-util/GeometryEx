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
    }
}
