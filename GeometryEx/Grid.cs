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
            Perimeter = perimeter ?? throw new ArgumentNullException(Messages.POLYGON_NULL_EXCEPTION);
            Position = position;
            IntervalX = intervalX != 0.0 ? Math.Abs(intervalX) : throw new ArgumentOutOfRangeException(Messages.ZERO_VALUE_EXCEPTION);
            IntervalY = intervalY != 0.0 ? Math.Abs(intervalY) : throw new ArgumentOutOfRangeException(Messages.ZERO_VALUE_EXCEPTION);
            var pivot = perimeter.Compass().C;
            var perimeterJig = perimeter.Rotate(pivot, angle * -1);
            compass = perimeterJig.Compass();
            var start = StartPoint();
            var anchorX = new Line(new Vector3(compass.W.X, start.Y), new Vector3(compass.E.X, start.Y));
            var anchorY = new Line(new Vector3(start.X, compass.S.Y), new Vector3(start.X, compass.N.Y));
            var gridX = new List<Line>();
            var gridY = new List<Line>();
            gridX.Add(anchorX);
            gridY.Add(anchorY);

            var mark = anchorX.Start.Y + IntervalY;
            while (mark <= compass.N.Y)
            {
                gridX.Add(new Line(new Vector3(compass.W.X, mark), new Vector3(compass.E.X, mark)));
                mark += IntervalY;
            }
            mark = anchorX.Start.Y - IntervalY;
            while (mark >= compass.S.Y)
            {
                gridX.Add(new Line(new Vector3(compass.W.X, mark), new Vector3(compass.E.X, mark)));
                mark -= IntervalY;
            }
            mark = anchorY.Start.X + IntervalX;
            while (mark <= compass.E.X)
            {
                gridY.Add(new Line(new Vector3(mark, compass.S.Y), new Vector3(mark, compass.N.Y)));
                mark += IntervalY;
            }
            mark = anchorY.Start.X - IntervalX;
            while (mark >= compass.W.X)
            {
                gridY.Add(new Line(new Vector3(mark, compass.S.Y), new Vector3(mark, compass.N.Y)));
                mark -= IntervalY;
            }

            LinesX = new List<Line>();
            LinesY = new List<Line>();

            foreach (var line in gridX)
            {
                LinesX.Add(line.Rotate(pivot, angle));
            }
            foreach (var line in gridY)
            {
                LinesY.Add(line.Rotate(pivot, angle));
            }
            LinesX = LinesX.OrderBy(g => g.Start.Y).ToList();
            LinesY = LinesY.OrderBy(g => g.Start.X).ToList();
        }

        #endregion

        #region Private

        private CompassBox compass;

        private Vector3 StartPoint ()
        {
            switch ((int)Position)
            {
                case (int)GridPosition.CenterSpan:
                    {
                        return compass.C.MoveFromTo(Vector3.Origin, new Vector3(IntervalX * -0.5, IntervalY * -0.5));
                    }
                case (int)GridPosition.CenterX:
                    {
                        return compass.C.MoveFromTo(Vector3.Origin, new Vector3(0.0, IntervalY * -0.5));
                    }
                case (int)GridPosition.CenterY:
                    {
                        return compass.C.MoveFromTo(Vector3.Origin, new Vector3(IntervalX * -0.5, 0.0));
                    }
                case (int)GridPosition.CenterXY:
                    {
                        return compass.C;
                    }
                case (int)GridPosition.MaxX:
                    {
                        return compass.C.MoveFromTo(Vector3.Origin, new Vector3(compass.SizeX * 0.5, IntervalY * -0.5));
                    }
                case (int)GridPosition.MaxY:
                    {
                        return compass.C.MoveFromTo(Vector3.Origin, new Vector3(IntervalX * -0.5, compass.SizeY * 0.5));
                    }
                case (int)GridPosition.MaxXY:
                    {
                        return compass.C.MoveFromTo(Vector3.Origin, new Vector3(compass.SizeX * 0.5, compass.SizeY * 0.5));
                    }
                case (int)GridPosition.MinX:
                    {
                        return compass.C.MoveFromTo(Vector3.Origin, new Vector3(compass.SizeX * -0.5, IntervalY * -0.5));
                    }
                case (int)GridPosition.MinY:
                    {
                        return compass.C.MoveFromTo(Vector3.Origin, new Vector3(IntervalX * -0.5, compass.SizeY * -0.5));
                    }
                case (int)GridPosition.MinXY:
                    {
                        return compass.C.MoveFromTo(Vector3.Origin, new Vector3(compass.SizeX * -0.5, compass.SizeY * -0.5));
                    }
            }
            return compass.C.MoveFromTo(Vector3.Origin, new Vector3(IntervalX * -0.5, IntervalY * -0.5));
        }

        #endregion

        #region Properties

        public double IntervalX { get; }
        public double IntervalY { get; }
        public List<Line> Lines
        {
            get
            {
                var lines = new List<Line>(LinesX);
                lines.AddRange(LinesY);
                return lines;
            }
        }

        public List<Line> LinesX { get; }

        public List<Line> LinesY { get; }

        /// <summary>
        /// Polygon perimeter of the grid. 
        /// </summary>
        public Polygon Perimeter { get; }

        public GridPosition Position { get; }

        #endregion

        #region Methods

        public Vector3 PointAt(int gridX, int gridY)
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

        #endregion

    }
}
