using System;
using System.Collections.Generic;
using System.Linq;
using Elements.Geometry;


namespace GeometryEx
{
    /// <summary>
    /// Creates a grid of lines and points defined by the bounding box of the supplied Polygon.
    /// </summary>
    public class Grid
    {

        #region Constructors

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
        public Grid(Polygon perimeter, double intervalX = 1.0, double intervalY = 1.0, 
                    double angle = 0.0, GridPosition position = GridPosition.CenterSpan)
        {

            if (intervalX <= 0.0 || intervalY <= 0.0)
            {
                throw new ArgumentOutOfRangeException(Messages.NON_POSITIVE_VALUE_EXCEPTION);
            }
            Perimeter = perimeter ?? throw new ArgumentNullException(Messages.POLYGON_NULL_EXCEPTION);
            IntervalX = intervalX;
            IntervalY = intervalY;
            Angle = angle;
            Position = position;
            perimeterJig = perimeter.Rotate(Vector3.Origin, angle * -1);
            compass = perimeterJig.Compass();
            var box = compass.Box;
            var origin = Origin();
            var thruPoints = new List<Vector3>() { origin };
            var point = new Vector3(origin.X + IntervalX, origin.Y + IntervalY);
            while (point.X < compass.E.X || point.Y < compass.N.Y)
            {
                thruPoints.Add(point);
                point = new Vector3(point.X + IntervalX, point.Y + IntervalY);
            }
            point = new Vector3(origin.X - IntervalX, origin.Y - IntervalY);
            while (point.X > compass.W.X || point.Y > compass.S.Y)
            {
                thruPoints.Add(point);
                point = new Vector3(point.X - IntervalX, point.Y - IntervalY);
            }
            LinesX = new List<Line>();
            LinesY = new List<Line>();
            foreach (var pnt in thruPoints)
            {
                if (pnt.Y > compass.S.Y && pnt.Y < compass.N.Y)
                {
                    LinesX.Add(new Line(new Vector3(compass.W.X, pnt.Y),
                                        new Vector3(compass.E.X, pnt.Y)));
                }
                if (pnt.X > compass.W.X && pnt.X < compass.E.X)
                {
                    LinesY.Add(new Line(new Vector3(pnt.X, compass.S.Y),
                                    new Vector3(pnt.X, compass.N.Y)));
                }
            }
            LinesX = LinesX.OrderBy(x => x.Start.Y).ToList();
            LinesY = LinesY.OrderBy(y => y.Start.X).ToList();
            MakeCells();
            RotateOrder();
        }

        #endregion

        #region Private

        private readonly double Angle;
        private readonly CompassBox compass;
        private readonly Polygon perimeterJig;

        /// <summary>
        /// 
        /// </summary>
        private void MakeCells()
        {
            var SW = new List<Vector3>()
            {
                compass.SW
            };
            SW.AddRange(StartsY);
            var NE = new List<Vector3>();
            for (var i = 0; i < LinesX.Count; i++)
            {
                SW.AddRange(PointsAlongX(i).SkipLast(1));
                NE.AddRange(PointsAlongX(i).Skip(1));
            }
            NE.AddRange(EndsY);
            NE.Add(compass.NE);
            SW = SW.Distinct().ToList();
            NE = NE.Distinct().ToList();
            Cells = new List<Polygon>();
            for (var i = 0; i < SW.Count; i++)
            {
                var points = new List<Vector3>()
                {
                    SW[i],
                    new Vector3(NE[i].X, SW[i].Y),
                    NE[i],
                    new Vector3(SW[i].X, NE[i].Y)
                };
                Cells.Add(new Polygon(points));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Vector3 Origin()
        {
            var origin = compass.C;
            var spanX = origin.X + (IntervalX * 0.5);
            var spanY = origin.Y + (IntervalY * 0.5);
            switch ((int)Position)
            {
                case (int)GridPosition.CenterSpan:
                    {
                        origin = new Vector3(spanX, spanY);
                        break;
                    }
                case (int)GridPosition.CenterX:
                    {
                        origin = new Vector3(origin.X, spanY);
                        break;
                    }
                case (int)GridPosition.CenterY:
                    {
                        origin = new Vector3(spanX, origin.Y);
                        break;
                    }
                case (int)GridPosition.MaxX:
                    {
                        origin = new Vector3(compass.E.X, spanY);
                        break;
                    }
                case (int)GridPosition.MaxY:
                    {
                        origin = new Vector3(spanX, compass.N.Y);
                        break;
                    }
                case (int)GridPosition.MaxXY:
                    {
                        origin = compass.NE;
                        break;
                    }
                case (int)GridPosition.MinX:
                    {
                        origin = new Vector3(compass.W.X, spanY);
                        break;
                    }
                case (int)GridPosition.MinY:
                    {
                        origin = new Vector3(spanX, compass.S.Y);
                        break;
                    }
                case (int)GridPosition.MinXY:
                    {
                        origin = compass.SW;
                        break;
                    }
            }
            if (!perimeterJig.Covers(origin))
            {
                return compass.C;
            }
            return origin;
        }

        private void RotateOrder()
        {
            var gridX = new List<Line>();
            var gridY = new List<Line>();
            foreach (var line in LinesX)
            {
                gridX.Add(line.Rotate(Vector3.Origin, Angle));
            }
            foreach (var line in LinesY)
            {
                gridY.Add(line.Rotate(Vector3.Origin, Angle));
            }
            LinesX = gridX.OrderBy(g => g.Start.Y).ToList();
            LinesY = gridY.OrderBy(g => g.Start.X).ToList();
            var gridCells = new List<Polygon>();
            foreach (var cell in Cells)
            {
                gridCells.Add(cell.Rotate(Vector3.Origin, Angle));
            }
            Cells = gridCells.OrderBy(c => c.Compass().C.X).ThenBy(c => c.Compass().C.Y).ToList();
        }

        #endregion Private

        #region Properties

        public List<Polygon> Cells { get; private set; }

        /// <summary>
        /// End points of X axes.
        /// </summary>
        public List<Vector3> EndsX
        {
            get
            {
                var points = new List<Vector3>();
                foreach (var line in LinesX)
                {
                    points.Add(line.End);
                }
                return points;
            }
        }

        /// <summary>
        /// Start points of Y axes.
        /// </summary>
        public List<Vector3> EndsY
        {
            get
            {
                var points = new List<Vector3>();
                foreach (var line in LinesY)
                {
                    points.Add(line.End);
                }
                return points;
            }
        }

        /// <summary>
        /// Returns all the points at the intersections of the grid lines.
        /// Points are supplied in sequential rows of X coordinates of increasing Y value.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> Intersections
        {
            get
            {
                var points = new List<Vector3>();
                for (var x = 0; x < LinesX.Count; x++)
                {
                    for (var y = 0; y < LinesY.Count; y++)
                    {
                        points.Add(Intersection(x, y));
                    }
                }
                return points;
            }
        }

        /// <summary>
        ///  Distance between X axis Grid lines.
        /// </summary>
        public double IntervalX { get; }

        /// <summary>
        ///  Distance between Y axis Grid lines.
        /// </summary>
        public double IntervalY { get; }

        /// <summary>
        /// List of all Grid lines.
        /// </summary>
        public List<Line> Lines
        {
            get
            {
                var lines = new List<Line>(LinesX);
                lines.AddRange(LinesY);
                return lines;
            }
        }

        /// <summary>
        /// List of X axis lines.
        /// </summary>
        public List<Line> LinesX { get; private set; }

        /// <summary>
        /// List of Y axis lines.
        /// </summary>
        public List<Line> LinesY { get; private set; }

        /// <summary>
        /// Polygon perimeter generating the Grid. 
        /// </summary>
        public Polygon Perimeter { get; private set; }

        /// <summary>
        /// Returns all the points at the ends and intersections of the grid lines.
        /// Points return in sequential rows of X coordinates of increasing Y value.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> Points
        {
            get
            {
                var points = new List<Vector3>(StartsY);
                for (var x = 0; x < LinesX.Count; x++)
                {
                    points.Add(LinesX[x].Start);
                    for (var y = 0; y < LinesY.Count; y++)
                    {
                        points.Add(Intersection(x, y));
                    }
                    points.Add(LinesX[x].End);
                }
                points.AddRange(EndsY);
                return points;
            }
        }

        /// <summary>
        /// Start position of the axis intervals.
        /// </summary>
        public GridPosition Position { get; }

        /// <summary>
        /// Start points of X axes.
        /// </summary>
        public List<Vector3> StartsX
        {
            get
            {
                var points = new List<Vector3>();
                foreach (var line in LinesX)
                {
                    points.Add(line.Start);
                }
                return points;
            }
        }

        /// <summary>
        /// Start points of Y axes.
        /// </summary>
        public List<Vector3> StartsY
        {
            get
            {
                var points = new List<Vector3>();
                foreach (var line in LinesY)
                {
                    points.Add(line.Start);
                }
                return points;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns the point at the intersection of the grid lines at the specified axis indices.
        /// Grid lines are numbered as two zero index lists from minimum to maximum X and Y coordinates.
        /// </summary>
        /// <param name="gridX">Index of a X axis grid line.</param>
        /// <param name="gridY">Index of a Y axis grid line.</param>
        /// <returns></returns>
        public Vector3 Intersection(int gridX, int gridY)
        {
            if (gridX < 0 || gridY < 0)
            {
                throw new ArgumentOutOfRangeException(Messages.NEGATIVE_VALUE_EXCEPTION);
            }
            if (gridX > LinesX.Count - 1 || gridY > LinesY.Count - 1)
            {
                return new Vector3(double.NaN, double.NaN, double.NaN);
            }
            var yLine = LinesY[gridY];
            var yPlane = new Plane(yLine.Start, yLine.End, yLine.End.MoveFromTo(Vector3.Origin, Vector3.ZAxis));
            LinesX[gridX].Intersects(yPlane, out Vector3 intersect);
            return intersect;
        }

        /// <summary>
        /// Returns the points at the ends and intersections an X grid line at the supplied index in order of ascending X value.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> PointsAlongX(int index)
        {
            if (index >= LinesX.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            var points = new List<Vector3>()
            {
                LinesX[index].Start
            };
            for (var i = 0; i < LinesY.Count; i++)
            {
                points.Add(Intersection(index, i));
            }
            points.Add(LinesX[index].End);
            return points;
        }

        /// <summary>
        /// Returns the points at the ends and intersections an Y grid line at the supplied index in order of ascending Y value.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> PointsAlongY(int index)
        {
            if (index >= LinesY.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            var points = new List<Vector3>()
            {
                LinesY[index].Start
            };
            for (var i = 0; i < LinesX.Count; i++)
            {
                points.Add(Intersection(i, index));
            }
            points.Add(LinesY[index].End);
            return points;
        }

        #endregion Methods

    }
}
