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
        /// Find the distance from this point to the line, and output the location 
        /// of the closest point on that line.
        /// Using formula from https://diego.assencio.com/?index=ec3d5dfdfc0b6a0d147a656f0af332bd
        /// </summary>
        /// <param name="line">The line to find the distance to.</param>
        /// <param name="closestPoint">The point on the line that is closest to this point.</param>
        public static double DistanceTo(this Vector3 point, Line line, out Vector3 closestPoint)
        {
            var lambda = 
                (point - line.Start).Dot(line.End - line.Start) / 
                (line.End - line.Start).Dot(line.End - line.Start);
            if (lambda >= 1)
            {
                closestPoint = line.End;
                return point.DistanceTo(line.End);
            }
            else if (lambda <= 0)
            {
                closestPoint = line.Start;
                return point.DistanceTo(line.Start);
            }
            else
            {
                closestPoint = (line.Start + lambda * (line.End - line.Start));
                return point.DistanceTo(closestPoint);
            }
        }

        /// <summary>
        /// Returns the Vector3 point in a List farthest from this point.
        /// </summary>
        /// <param name="points">List of Vector3 points to compare.</param>
        /// <returns>
        /// A Vector3 point.
        /// </returns>
        public static Vector3 FarthestFrom (this Vector3 point, List<Vector3> points)
        {
            if (points.Count == 0)
            {
                return new Vector3(double.NaN, double.NaN);
            }
            return points.OrderByDescending(pnt => point.DistanceTo(pnt)).First();
        }

        /// <summary>
        /// Return true if an AlmostEqual Vector3 point appears at least once in the supplied list.
        /// </summary>
        /// <returns>
        /// True if this Vector3 point IsAlmostEqual to a Vector3 point in the supplied List.
        /// </returns>
        public static bool IsListed(this Vector3 point, List<Vector3> points)
        {
            foreach (var entry in points)
            {
                if (entry.IsAlmostEqualTo(point))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the Vector3 point moved to a new relative location.
        /// </summary>
        /// <param name="from">Vector3 base point of the move.</param>
        /// <param name="to">Vector3 target point of the move.</param>
        /// <returns>
        /// A new Vector3 points.
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
        /// A Vector3 point.
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
        /// Return true if an NearEqual Vector3 appears in the supplied list.
        /// </summary>
        /// <param name="points">List of Vector3 to compare to point.</param>
        /// <returns>
        /// True if this Vectors 3 appears in the supplied List of Vector3 points.
        /// </returns>
        public static int Occurs(this Vector3 point, List<Vector3> points)
        {
            var occurs = 0;
            foreach (var entry in points)
            {
                if (point.IsAlmostEqualTo(entry))
                {
                    occurs++;
                }
            }
            return occurs;
        }

        /// <summary>
        /// Returns a new Vector3 rotated around a supplied Vector3 by the specified angle in degrees.
        /// </summary>
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

        /// <summary>
        /// Inserts this Vector3 into a new List.
        /// </summary>
        /// <returns>A List containing this Vector3.</returns>
        public static List<Vector3> ToList(this Vector3 vector)
        {
            return new List<Vector3> { vector };
        }
    }
}
