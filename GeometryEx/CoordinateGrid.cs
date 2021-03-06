﻿using System;
using System.Collections.Generic;
using System.Linq;
using Elements.Geometry;

namespace GeometryEx
{
    /// <summary>
    /// Maintains a list of available and allocated points in a grid of the specified interval within the orthogonal bounding box of a Polygon.
    /// </summary>
    public class CoordinateGrid
    {
        /// <summary>
        /// Providing a random seed value ensures reproducible results.
        /// </summary>
        private Random random;

        /// <summary>
        /// The list of vector3 allocated points.
        /// </summary>
        public List<Vector3> Allocated { get; }

        /// <summary>
        /// The list of Vector3 points available for allocation.
        /// </summary>
        public List<Vector3> Available { get; }

        /// <summary>
        /// Polygon perimeter of the grid. 
        /// </summary>
        private Polygon perimeter;
        public Polygon Perimeter
        {
            get { return perimeter; }
            set
            {
                if (value != null)
                {
                    perimeter = value;
                }
            }
        }

        /// <summary>
        /// Creates a rotated 2D grid of Vector3 points from the supplied Polygon, axis intervals, and angle.
        /// </summary>
        /// <param name="perimeter">Polygon boundary of the point grid.</param>
        /// <param name="xInterval">Spacing of the grid along the x-axis.</param>
        /// <param name="yInterval">Spacing of the grid along the y-axis.</param>
        /// <param name="angle">Rotation of the grid around the Polygon centroid.</param>
        /// <returns>
        /// A new CoordGrid.
        /// </returns>
        public CoordinateGrid(Polygon polygon, double xInterval = 1.0,  double yInterval = 1.0, double angle = 0.0)
        {
            if (polygon == null)
            {
                return;
            }
            random = new Random();
            Allocated = new List<Vector3>();
            Available = new List<Vector3>();
            Perimeter = Shaper.MakePolygon(polygon.Vertices.ToList());
            var centroid = polygon.Centroid();
            var box = new CompassBox(polygon);
            var points = new List<Vector3>();

            // Northeast quadrant
            var x = centroid.X + (xInterval * 0.5);
            var y = centroid.Y + (yInterval * 0.5);
            while (y <= box.NW.Y)
            {
                while (x <= box.SE.X)
                {
                    points.Add(new Vector3(x, y));
                    x += xInterval;
                }
                x = centroid.X + (xInterval * 0.5);
                y += yInterval;
            }

            // Northwest quadrant
            x = centroid.X - (xInterval * 0.5);
            y = centroid.Y + (yInterval * 0.5);
            while (y <= box.NW.Y)
            {
                while (x >= box.SW.X)
                {
                    points.Add(new Vector3(x, y));
                    x -= xInterval;
                }
                x = centroid.X - (xInterval * 0.5);
                y += yInterval;
            }

            // Southeast quadrant
            x = centroid.X + (xInterval * 0.5);
            y = centroid.Y - (yInterval * 0.5);
            while (y >= box.SW.Y)
            {
                while (x <= box.SE.X)
                {
                    points.Add(new Vector3(x, y));
                    x += xInterval;
                }
                x = centroid.X + (xInterval * 0.5);
                y -= yInterval;
            }

            // Southwest quadrant
            x = centroid.X - (xInterval * 0.5);
            y = centroid.Y - (yInterval * 0.5);
            while (y >= box.SW.Y)
            {
                while (x >= box.SW.X)
                {
                    points.Add(new Vector3(x, y));
                    x -= xInterval;
                }
                x = centroid.X - (xInterval * 0.5);
                y -= yInterval;
            }
            foreach (var pnt in points)
            {
                var point = pnt.Rotate(centroid, angle);
                if (polygon.Covers(point))
                {
                    Available.Add(point);
                }
            }
        }

        /// <summary>
        /// Allocates the points in the grid falling within or on the supplied Polygon.
        /// </summary>
        /// <param name="polygon">The Polygon bounding the points to be allocated.</param>
        /// <returns>
        /// None.
        /// </returns>
        public void Allocate(Polygon polygon)
        {
            if (polygon == null)
            {
                return;
            }
            var rmvPoints = new List<int>();
            var index = 0;
            foreach (Vector3 point in Available)
            {
                if (polygon.Covers(point))
                {
                    rmvPoints.Add(index);
                    Allocated.Add(point);
                }
                index++;
            }
            rmvPoints.Reverse();
            foreach(int rmv in rmvPoints)
            {
                Available.RemoveAt(rmv);
            }
        }

        /// <summary>
        /// Allocates points in the grid falling within the supplied Polygons.
        /// </summary>
        /// <param name="polygon">The Polygon bounding the points to be allocated.</param>
        /// <returns>
        /// None.
        /// </returns>
        public void Allocate(List<Polygon> polygons)
        {
            if (polygons.Count == 0)
            {
                return;
            }
            var index = 0;
            var allocate = new List<Vector3>();
            foreach (Vector3 point in Available)
            {
                foreach (Polygon polygon in polygons)
                {
                    if (polygon.Covers(point))
                    {
                        allocate.Add(point);
                    }
                    index++;
                }
            }
            foreach (Vector3 point in allocate)
            {
                Available.Remove(point);
                Allocated.Add(point);
            }
        }

        /// <summary>
        /// Returns the allocated grid point nearest to the supplied point.
        /// </summary>
        /// <param name="point">The Vector3 point to compare.</param>
        /// <returns>
        /// A Vector3 point.
        /// </returns>
        public Vector3 AllocatedNearTo(Vector3 point)
        {
            if (Allocated.Count == 0)
            {
                return new Vector3(double.NaN, double.NaN, double.NaN);
            }
            var x = Allocated.First().X;
            var y = Allocated.First().Y;
            foreach (Vector3 aPoint in Allocated)
            {
                var xDelta = Math.Abs(point.X - aPoint.X);
                var yDelta = Math.Abs(point.Y - aPoint.Y);
                if (xDelta <= Math.Abs(x - aPoint.X) ||
                    yDelta <= Math.Abs(y - aPoint.Y))
                {
                    x = aPoint.X;
                    y = aPoint.Y;
                }
            }
            return new Vector3(x, y);
        }

        /// <summary>
        /// Returns a random allocated point.
        /// </summary>
        /// <returns>
        /// A Vector3 point.
        /// </returns>
        public Vector3 AllocatedRandom()
        {
            if (Allocated.Count == 0)
            {
                return new Vector3(double.NaN, double.NaN, double.NaN);
            }
            return Allocated[random.Next(0, Allocated.Count - 1)];
        }

        /// <summary>
        /// Returns the maximum available grid point.
        /// </summary>
        /// <returns>
        /// A Vector3 point.
        /// </returns>
        public Vector3 AvailableMax()
        {
            if (Available.Count == 0)
            {
                return new Vector3(double.NaN, double.NaN, double.NaN);
            }
            var maxX = Available.First().X;
            var maxY = Available.First().Y;
            foreach (Vector3 point in Available)
            {
                if (point.X + point.Y > maxX + maxY)
                {
                    maxX = point.X;
                    maxY = point.Y;
                }
            }
            return new Vector3(maxX, maxY);
        }

        /// <summary>
        /// Returns the minimum available grid point.
        /// </summary>
        /// <returns>
        /// A Vector3 point.
        /// </returns>

        public Vector3 AvailableMin()
        {
            if (Available.Count == 0)
            {
                return new Vector3(double.NaN, double.NaN, double.NaN);
            }
            var minX = Available.First().X;
            var minY = Available.First().Y;
            foreach (Vector3 point in Available)
            {
                if (point.X + point.Y < minX + minY)
                {
                    minX = point.X;
                    minY = point.Y;
                }
            }
            return new Vector3(minX, minY);
        }

        /// <summary>
        /// Returns the available grid point nearest to the supplied Vector3 point.
        /// </summary>
        /// <param name="point">The Vector3 point to compare.</param>
        /// <returns>
        /// A Vector3 point.
        /// </returns>
        public Vector3 AvailableNearTo(Vector3 point)
        {
            if (Available.Count == 0)
            {
                return new Vector3(double.NaN, double.NaN, double.NaN);
            }
            var x = Available.First().X;
            var y = Available.First().Y;
            foreach (Vector3 aPoint in Available)
            {
                var xDelta = Math.Abs(point.X - aPoint.X);
                var yDelta = Math.Abs(point.Y - aPoint.Y);
                if (xDelta <= Math.Abs(x - aPoint.X) ||
                    yDelta <= Math.Abs(y - aPoint.Y))
                {
                    x = aPoint.X;
                    y = aPoint.Y;
                }

            }
            return new Vector3(x, y);
        }

        /// <summary>
        /// Returns a random available grid point.
        /// </summary>
        /// <returns>
        /// A Vector3 point.
        /// </returns>
        public Vector3 AvailableRandom()
        {
            if (Available.Count == 0)
            {
                return new Vector3(double.NaN, double.NaN, double.NaN);
            }
            return Available[random.Next(0, Available.Count - 1)];
        }
    }
}
