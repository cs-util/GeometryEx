using System;
using System.Linq;
using System.Collections.Generic;
using Elements.Geometry;

namespace GeometryEx
{
    /// <summary>
    /// Extends Elements.Geometry.Mesh with utility methods.
    /// </summary>
    public static class TriangleEx
    {
        /// <summary>
        /// Returns the area of the triangle.
        /// </summary>
        /// <returns>A double.</returns>
        public static double Area(this Triangle triangle)
        {
            var a = triangle.Vertices[0].Position;
            var b = triangle.Vertices[1].Position;
            var c = triangle.Vertices[2].Position;

            return Math.Abs((a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y)) * 0.5);
        }

        /// <summary>
        /// Returns the edges of a Triangle as a List of Lines.
        /// </summary>
        /// <returns>A List of Lines.</returns>
        public static List<Line> Edges(this Triangle triangle)
        {
            var edges = new List<Line>();
            var points = new List<Vector3>();
            triangle.Vertices.ToList().ForEach(v => points.Add(v.Position));
            edges.Add(new Line(points[0], points[1]));
            edges.Add(new Line(points[1], points[2]));
            edges.Add(new Line(points[2], points[0]));
            return edges;
        }


    }
}
