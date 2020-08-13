using System;
using System.Collections.Generic;
using Elements.Geometry;

namespace GeometryEx
{
    /// <summary>
    /// Extends Elements.Geometry.Arc with utility methods.
    /// </summary>
    public static class ArcEx
    {
        /// <summary>
        /// Creates a collection of Vector3 points representing the division of the linear geometry into the supplied number of segments.
        /// </summary>
        /// <param name="segments">Quantity of desired segments.</param>
        /// <returns>
        /// A List of Vector3 points.
        /// </returns>
        public static List<Vector3> Divide(this Arc arc, int segments)
        {
            var pointList = new List<Vector3>()
            {
                arc.Start
            };
            var percent = 1.0 / segments;
            var factor = 1;
            var at = percent * factor;
            for (int i = 0; i < segments; i++)
            {
                pointList.Add(arc.PointAt(at));
                at = percent * ++factor;
            }
            return pointList;
        }

        /// <summary>
        /// Returns a List of Lines representing the Arc divided into the specified quantity of segments.
        /// </summary>
        /// <param name="segments">Quantity of desired segments.</param>
        /// <returns>
        /// A List of Lines.
        /// </returns>
        public static List<Line> ToLines(this Arc arc, int segments)
        {
            return Shaper.PointsToLines(arc.Divide(segments));
        }

        /// <summary>
        /// Returns a Polyline representing the Arc divided into the specified quantity of segments.
        /// </summary>
        /// <param name="segments">Quantity of desired segments.</param>
        /// <returns>
        /// A new Polyline.
        /// </returns>
        public static Polyline ToPolyline(this Arc arc, int segments)
        {
            return new Polyline(arc.Divide(segments));
        }
    }
}
