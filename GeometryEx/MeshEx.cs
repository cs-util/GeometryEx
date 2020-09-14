using System;
using System.Linq;
using System.Collections.Generic;
using Elements.Geometry;

namespace GeometryEx
{
    /// <summary>
    /// Extends Elements.Geometry.Mesh with utility methods.
    /// </summary>
    public static class MeshEx
    {
        /// <summary>
        /// Returns the aggregate area of all Mesh triangles.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns>
        /// A double.
        /// </returns>
        public static double Area (this Mesh mesh)
        {
            var area = 0.0;
            mesh.Triangles.ForEach(t => area += t.Area());
            return area;
        }

        /// <summary>
        /// Returns the Mesh Triangle(s) that border the supplied Line edge.
        /// </summary>
        /// <param name="edge">A Line representing a Mesh edge.</param>
        /// <returns>
        /// A List of Triangles.
        /// </returns>
        public static List<Triangle> AdjacentTriangles(this Mesh mesh, Line edge)
        {
            var triangles = new List<Triangle>();
            foreach (var triangle in mesh.Triangles)
            {
                var points = new List<Vector3>();
                triangle.Vertices.ToList().ForEach(v => points.Add(v.Position));
                if (points.Contains(edge.Start) && points.Contains(edge.End))
                {
                    triangles.Add(triangle);
                }
            }
            return triangles;
        }

        /// <summary>
        /// Averages the vectors terminating at a Mesh Vertex..
        /// </summary>
        /// <param name="point">A Vector3 at the same position as a Vertex on the Mesh.</param>
        /// <param name="orient">If true, returns lines whose end point is at the supplied point.</param>
        /// <returns>
        /// A Vector3 average.
        /// </returns>
        public static Vector3 AverageAt(this Mesh mesh, Vector3 point)
        {
            var average = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);
            var points = mesh.Points();
            if (!points.Any(p => p.IsAlmostEqualTo(point)))
            {
                return average;
            }
            var edges = EdgesAt(mesh, point);
            var vectors = new List<Vector3>();
            edges.ForEach(e => vectors.Add(new Vector3(e.End.X - e.Start.X, e.End.Y - e.Start.Y, e.End.Z - e.Start.Z).Unitized()));
            average = vectors[0].Average(vectors[1]);
            for (int i = 2; i < vectors.Count; i++)
            {
                average = average.Average(vectors[i]);
            }
            return average.Unitized();
        }
        /// <summary>
        /// Returns the unique edges of a Mesh as a List of Lines.
        /// </summary>
        /// <returns>A List of Lines.</returns>
        public static List<Line> Edges(this Mesh mesh)
        {
            var edges = new List<Line>();
            mesh.Triangles.ForEach(t => edges.AddRange(t.Edges().Where(e => !e.IsListed(edges))));
            return edges;
        }

        /// <summary>
        /// Returns the unique edges intersecting a supplied point of the Mesh, or an empty list if the supplied point is not at the same position as a Mesh Vertex.
        /// </summary>
        /// <param name="point">A Vector3 at the same position as a Vertex on the Mesh.</param>
        /// <param name="orient">If true, returns lines whose end point is at the supplied point.</param>
        /// <returns>
        /// A List of Lines.
        /// </returns>
        public static List<Line> EdgesAt(this Mesh mesh, Vector3 point, bool orient = true)
        {
            var lines = new List<Line>();
            var points = mesh.Points();
            if (!points.Any(p => p.IsAlmostEqualTo(point)))
            {
                return lines;
            }
            mesh.Triangles.ForEach(t => lines.AddRange(t.Edges().Where(e => !e.IsListed(lines))));
            lines = lines.Where(e => e.Start.IsAlmostEqualTo(point) || e.End.IsAlmostEqualTo(point)).ToList();
            if (!orient)
            {
                return lines;
            }
            var edges = new List<Line>();
            foreach (var line in lines)
            {
                if (line.End.IsAlmostEqualTo(point))
                {
                    edges.Add(line);
                    continue;
                }
                edges.Add(new Line(line.End, line.Start));
            }
            return edges;
        }

        /// <summary>
        /// Returns whether all normals of this Mesh's triagles are equivalent.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns>
        /// True if the absolute value of all normals is equivalent.
        /// </returns>
        public static bool IsFlat(this Mesh mesh)
        {
            var triangles = mesh.Triangles;
            var idx = triangles.First().Normal;
            var index = new Vector3(Math.Abs(idx.X), Math.Abs(idx.Y), Math.Abs(idx.Z));
            foreach (var triangle in triangles)
            {
                var vtx = triangle.Normal;
                var compare = new Vector3(Math.Abs(vtx.X), Math.Abs(vtx.Y), Math.Abs(vtx.Z));
                if (!index.IsAlmostEqualTo(compare))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Calculates whether a supplied edge of the Mesh is a valley relative to its adjacent triangles and a supplied comparison vector.
        /// </summary>
        /// <param name="edge">A Line representing an edge of this Mesh.</param>
        /// <param name="compareTo">A vector to compare adjacent triangle normals.</param>
        /// <returns></returns>
        public static bool IsValley(this Mesh mesh, Line edge, Vector3 compareTo)
        {
            compareTo = compareTo.Unitized();
            var triangles = mesh.AdjacentTriangles(edge);
            if (triangles.Count == 0) //Line is not a Mesh edge.
            {
                return false;
            }
            var origin = triangles[0].Points().Where(p => !p.IsAlmostEqualTo(edge.Start) &&
                                                          !p.IsAlmostEqualTo(edge.End)).First();

            var nrmPlane = new Plane(origin, compareTo);
            var distS0 = nrmPlane.SignedDistanceTo(edge.Start);
            var distE0 = nrmPlane.SignedDistanceTo(edge.End);
            if (triangles.Count == 1)
            {
                if (distS0 < 0.0 && distE0 < 0.0)
                {
                    return true;
                }
                return false;
            }
            origin = triangles[1].Points().Where(p => !p.IsAlmostEqualTo(edge.Start) &&
                                                      !p.IsAlmostEqualTo(edge.End)).First();
            nrmPlane = new Plane(origin, compareTo);
            var distS1 = nrmPlane.SignedDistanceTo(edge.Start);
            var distE1 = nrmPlane.SignedDistanceTo(edge.End);
            if (distS0 < 0.0 && distE0 < 0.0 &&
                distS1 < 0.0 && distE1 < 0.0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a list of distict points in the Mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns>
        /// A Vector3 List.
        /// </returns>
        public static List<Vector3> Points(this Mesh mesh)
        {
            var triangles = mesh.Triangles;
            var points = new List<Vector3>();
            triangles.ForEach(t => points.AddRange(t.Points()));
            return points.Distinct().ToList();
        }

        public enum Normal { X, Y, Z }
        /// <summary>
        /// Returns a Plane for every distinct specified coordinate in this Mesh.
        /// </summary>
        /// <returns>
        /// A List of Planes in ascending axis order.
        /// </returns>
        public static List<Plane> Planes(this Mesh mesh, Normal axis)
        {
            var slices = new List<double>();
            var planes = new List<Plane>();
            var points = new List<Vector3>();
            mesh.Triangles.ForEach(t => t.Vertices.ToList().ForEach(v => points.Add(v.Position)));
            switch (axis)
            {
                case Normal.X:
                    points.ForEach(v => slices.Add(v.X));
                    slices.Distinct().ToList().ForEach(s => planes.Add(new Plane(new Vector3(s, 0.0, 0.0), Vector3.XAxis)));
                    planes = planes.OrderBy(p => p.Origin.X).ToList();
                    break;
                case Normal.Y:
                    points.ForEach(v => slices.Add(v.Y));
                    slices.Distinct().ToList().ForEach(s => planes.Add(new Plane(new Vector3(0.0, s, 0.0), Vector3.YAxis)));
                    planes = planes.OrderBy(p => p.Origin.Y).ToList();
                    break;
                case Normal.Z:
                    points.ForEach(v => slices.Add(v.Z));
                    slices.Distinct().ToList().ForEach(s => planes.Add(new Plane(new Vector3(0.0, 0.0, s), Vector3.ZAxis)));
                    planes = planes.OrderBy(p => p.Origin.Z).ToList();
                    break;
            }
            return planes; ;
        }
    }
}
