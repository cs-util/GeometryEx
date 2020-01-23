using System;
using System.Collections.Generic;
using System.Linq;
using Elements.Geometry;

namespace GeometryEx
{
    /// <summary>
    /// Extends Elements.Geometry.Vector3 with utility methods.
    /// </summary>
    public static class Vector3Ex
    {
        /// <summary>
        /// Returns the Vector3 point in a List farthest from this point.
        /// </summary>
        /// <param name="points">List of Vector3 points to compare.</param>
        /// <returns>
        /// A Vector3.
        /// </returns>
        public static Vector3 FarthestFrom (this Vector3 point, IList<Vector3> points)
        {
            if (points.Count == 0)
            {
                return new Vector3(double.NaN, double.NaN);
            }
            return points.OrderByDescending(pnt => point.DistanceTo(pnt)).First();
        }

        /// <summary>
        /// Returns the Vector3 point moved to a new relative location.
        /// </summary>
        /// <param name="vector">This Vector3.</param>
        /// <param name="from">Vector3 base point of the move.</param>
        /// <param name="to">Vector3 target point of the move.</param>
        /// <returns>
        /// A Vector3.
        /// </returns>
        public static Vector3 MoveFromTo(this Vector3 vector, Vector3 from, Vector3 to)
        {
            var t = new Transform();
            t.Move(new Vector3(to.X - from.X, to.Y - from.Y, to.Z - from.Z));
            return t.OfPoint(vector);
        }

        /// <summary>
        /// Returns the Vector3 point in a List closest to this point.
        /// </summary>
        /// <param name="points">List of Vector3 points to compare.</param>
        /// <returns>
        /// A Vector3.
        /// </returns>
        public static Vector3 NearestTo(this Vector3 point, IList<Vector3> points)
        {
            if (points.Count == 0)
            {
                return new Vector3(double.NaN, double.NaN);
            }
            return points.OrderBy(pnt => point.DistanceTo(pnt)).First();
        }

        /// <summary>
        /// Returns a new Vector3 rotated around a supplied Vector3 by the specified angle in degrees.
        /// </summary>
        /// <param name="point">The Vector3 instance to be rotated.</param>
        /// <param name="pivot">The Vector3 base point of the rotation.</param>
        /// <param name="angle">The desired rotation angle in degrees.</param>
        /// <returns>
        /// A new Vector3.
        /// </returns>
        public static Vector3 Rotate(this Vector3 point, Vector3 pivot, double angle)
        {
            var theta = angle * (Math.PI / 180);
            var rX = (Math.Cos(theta) * (point.X - pivot.X)) - (Math.Sin(theta) * (point.Y - pivot.Y)) + pivot.X;
            var rY = (Math.Sin(theta) * (point.X - pivot.X)) + (Math.Cos(theta) * (point.Y - pivot.Y)) + pivot.Y;
            return new Vector3(rX, rY);
        }
    }
}
