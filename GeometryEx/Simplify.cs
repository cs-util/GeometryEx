using System;
using System.Collections.Generic;
using Elements.Geometry;

namespace GeometryEx
{
    //Copyright(c) 2018, Magnus Henning
    //All rights reserved.

    //Redistribution and use in source and binary forms, with or without
    //modification, are permitted provided that the following conditions are met:

    //* Redistributions of source code must retain the above copyright notice, this
    //  list of conditions and the following disclaimer.

    //* Redistributions in binary form must reproduce the above copyright notice,
    //  this list of conditions and the following disclaimer in the documentation
    //  and/or other materials provided with the distribution.

    //THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    //AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    //IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    //DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
    //FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    //DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    //SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    //CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    //OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    //OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
    //© 2020 GitHub, Inc.

    //Minor modifications by Anthony A. Hauck for Hypar.Elements compatibility.
    public static class SimplifyNet
    {

        // square distance between 2 points
        private static double GetSqDist(Vector3 p1, Vector3 p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;

            return (dx * dx) + (dy * dy);
        }

        // square distance from a point to a segment
        private static double GetSqSegDist(Vector3 p, Vector3 p1, Vector3 p2)
        {
            double x = p1.X;
            double y = p1.Y;
            double dx = p2.X - x;
            double dy = p2.Y - y;

            if (Math.Abs(dx) > 0 || Math.Abs(dy) > 0)
            {
                double t = ((p.X - x) * dx + (p.Y - y) * dy) / (dx * dx + dy * dy);

                if (t > 1)
                {
                    x = p2.X;
                    y = p2.Y;
                }
                else if (t > 0)
                {
                    x += dx * t;
                    y += dy * t;
                }
            }

            dx = p.X - x;
            dy = p.Y - y;

            return dx * dx + dy * dy;
        }

        // basic distance-based simplification
        private static List<Vector3> SimplifyRadialDist(List<Vector3> points, double sqTolerance)
        {
            var point = Vector3.Origin;
            Vector3 prevPoint = points[0];
            var newPoints = new List<Vector3>() { prevPoint };

            for (int i = 1, len = points.Count; i < len; i++)
            {
                point = points[i];
                if (GetSqDist(point, prevPoint) > sqTolerance)
                {
                    newPoints.Add(point);
                    prevPoint = point;
                }
            }

            if (!prevPoint.IsAlmostEqualTo(point))
            {
                newPoints.Add(point);
            }

            return newPoints;
        }

        private static void SimplifyDpStep(List<Vector3> points, int first, int last, double sqTolerance, List<Vector3> simplified)
        {
            double maxSqDist = sqTolerance;
            var index = 0;

            for (int i = first + 1; i < last; i++)
            {
                double sqDist = GetSqSegDist(points[i], points[first], points[last]);

                if (sqDist > maxSqDist)
                {
                    index = i;
                    maxSqDist = sqDist;
                }
            }

            if (maxSqDist > sqTolerance)
            {
                if (index - first > 1)
                {
                    SimplifyDpStep(points, first, index, sqTolerance, simplified);

                }

                simplified.Add(points[index]);

                if (last - index > 1)
                {
                    SimplifyDpStep(points, index, last, sqTolerance, simplified);
                }
            }
        }

        // simplification using Ramer-Douglas-Peucker algorithm
        private static List<Vector3> SimplifyDouglasPeucker(List<Vector3> points, double sqTolerance)
        {
            int last = points.Count - 1;

            var simplified = new List<Vector3>() { points[0] };
            SimplifyDpStep(points, 0, last, sqTolerance, simplified);
            simplified.Add(points[last]);

            return simplified;
        }


        // both algorithms combined for awesome performance
        public static List<Vector3> Simplify(List<Vector3> points, double tolerance = 1, bool highestQuality = false)
        {
            if (points.Count <= 2)
            {
                return points;
            }

            double sqTolerance = tolerance * tolerance;

            points = highestQuality ? points : SimplifyRadialDist(points, sqTolerance);
            points = SimplifyDouglasPeucker(points, sqTolerance);

            return points;
        }
    }
}
