using System;
using System.Collections.Generic;
using System.Linq;
using Elements.Geometry;


namespace GeometryEx
{

    // Convex hull algorithm - Library (C#)
    // 
    // Copyright (c) 2017 Project Nayuki
    // https://www.nayuki.io/page/convex-hull-algorithm
    // 
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU Lesser General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    // 
    // This program is distributed in the hope that it will be useful,
    // but WITHOUT ANY WARRANTY; without even the implied warranty of
    // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    // GNU Lesser General Public License for more details.
    // 
    // You should have received a copy of the GNU Lesser General Public License
    // along with this program (see COPYING.txt and COPYING.LESSER.txt).
    // If not, see <http://www.gnu.org/licenses/>.

    // Uses Andrew's monotone chain algorithm. Positive y coordinates correspond to "up"
    // as per the mathematical convention, instead of "down" as per the computer
    // graphics convention. This doesn't affect the correctness of the result.
 
    // This code has been refactored into one method and altered to return Hypar Element Vector3 types.

    public static class ConvexHull
    {
        /// <summary>
        /// Returns a List of Vector3 points representing the counterclockwise convex hull of the supplied List of Vector3 points.
        /// The returned convex hull excludes collinear points.
        /// This algorithm runs in O(n log n) time.
        /// </summary>
        /// <param name="points">A List of Vector3 points</param>
        /// <returns>A List of Vector3 points</returns>

        public static List<Vector3> MakeHull(List<Vector3> vertices)
        {
            if (vertices.Count <= 1)
            {
                return vertices;
            }
            var points = vertices.Select(p => new Point(p.X, p.Y)).ToList();
            points.Sort();

            var upperHull = new List<Point>();
            foreach (var p in points)
            {
                while (upperHull.Count >= 2)
                {
                    var q = upperHull[upperHull.Count - 1];
                    var r = upperHull[upperHull.Count - 2];
                    if ((q.x - r.x) * (p.y - r.y) >= (q.y - r.y) * (p.x - r.x))
                    {
                        upperHull.RemoveAt(upperHull.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }
                upperHull.Add(p);
            }
            upperHull.RemoveAt(upperHull.Count - 1);

            var lowerHull = new List<Point>();
            for (int i = points.Count - 1; i >= 0; i--)
            {
                var p = points[i];
                while (lowerHull.Count >= 2)
                {
                    var q = lowerHull[lowerHull.Count - 1];
                    var r = lowerHull[lowerHull.Count - 2];
                    if ((q.x - r.x) * (p.y - r.y) >= (q.y - r.y) * (p.x - r.x))
                    {
                        lowerHull.RemoveAt(lowerHull.Count - 1);
                    }
                    else
                    {
                        break;
                    }
                }
                lowerHull.Add(p);
            }
            lowerHull.RemoveAt(lowerHull.Count - 1);

            if (!(upperHull.Count == 1 && Enumerable.SequenceEqual(upperHull, lowerHull)))
            {
                upperHull.AddRange(lowerHull);
            }
            var hull = upperHull.Select(p => new Vector3(p.x, p.y)).Distinct().ToList();
            hull.Reverse();
            return hull;
        }

        public struct Point : IComparable<Point>
        {

            public double x;
            public double y;

            public Point(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            public int CompareTo(Point other)
            {
                if (x < other.x)
                    return -1;
                else if (x > other.x)
                    return +1;
                else if (y < other.y)
                    return -1;
                else if (y > other.y)
                    return +1;
                else
                    return 0;
            }
        }
    }

}