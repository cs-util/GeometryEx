using System;
using Xunit;
using Elements.Geometry;
using GeometryEx;


namespace GeometryExTests
{
    public class LineExTests
    {
        [Fact]
        public void Divide()
        {
            var line = new Line(Vector3.Origin, new Vector3(0.0, 150.0));
            Assert.Equal(25.0, line.Divide(24).Count);
        }

        [Fact]
        public void Intersection()
        {
            var line = new Line(new Vector3(1.0, 1.0), new Vector3(6.0, 6.0));
            var intr = new Line(new Vector3(1.0, 5.0), new Vector3(5.0, 1.0));
            var point = line.Intersection(intr);
            Assert.Equal(3.0, point.X);
            Assert.Equal(3.0, point.Y);
            line = new Line(new Vector3(1.0, 3.0), new Vector3(5.0, 7.0));
            intr = new Line(new Vector3(3.0, 1.0), new Vector3(3.0, 7.0));
            point = line.Intersection(intr);
            Assert.Equal(3.0, point.X);
            Assert.Equal(5.0, point.Y);
            line = new Line(new Vector3(2.0, 4.0), new Vector3(9.0, 4.0));
            intr = new Line(new Vector3(3.0, 1.0), new Vector3(8.0, 6.0));
            point = line.Intersection(intr);
            Assert.Equal(6.0, point.X);
            Assert.Equal(4.0, point.Y);
        }

        [Fact]
        public void IsContiguousWith()
        {
            var line = new Line(new Vector3(1.0, 1.0), new Vector3(6.0, 6.0));
            var intr = new Line(new Vector3(6.0, 6.0), new Vector3(12.0, 12.0));
            Assert.True(line.IsContiguousWith(intr));

            line = new Line(new Vector3(1.0, 1.0), new Vector3(6.0, 6.0));
            intr = new Line(new Vector3(7.0, 7.0), new Vector3(12.0, 12.0));
            Assert.False(line.IsContiguousWith(intr));

            line = new Line(new Vector3(1.0, 1.0), new Vector3(6.0, 6.0));
            intr = new Line(new Vector3(6.0, 6.0), new Vector3(13.0, 12.0));
            Assert.False(line.IsContiguousWith(intr));
        }

        [Fact]
        public void IsHorizontal()
        {
            var line = new Line(new Vector3(1.0, 1.0), new Vector3(6.0, 6.0));
            Assert.False(line.IsHorizontal());

            line = new Line(new Vector3(1.0, 1.0), new Vector3(6.0, 1.0));
            Assert.True(line.IsHorizontal());
        }

        [Fact]
        public void IsParallelTo()
        {
            var line = new Line(new Vector3(1.0, 1.0), new Vector3(6.0, 6.0));
            var intr = new Line(new Vector3(5.0, 5.0), new Vector3(8.0, 8.0));
            Assert.True(line.IsParallelTo(intr));

            line = new Line(new Vector3(1.0, 1.0), new Vector3(6.0, 6.0));
            intr = new Line(new Vector3(5.0, 5.0), new Vector3(8.0, 9.0));
            Assert.False(line.IsParallelTo(intr));
        }

        [Fact]
        public void IsVertical()
        {
            var line = new Line(new Vector3(1.0, 1.0), new Vector3(6.0, 6.0));
            Assert.False(line.IsVertical());

            line = new Line(new Vector3(1.0, 1.0), new Vector3(1.0, 5.0));
            Assert.True(line.IsVertical());
        }

        [Fact]
        public void MoveFromTo()
        {
            var line = new Line(Vector3.Origin, new Vector3(0.0, 150));
            var moved = line.MoveFromTo(Vector3.Origin, new Vector3(0.0, 150.0));
            Assert.Equal(0.0, moved.End.X);
            Assert.Equal(300.0, moved.End.Y);
        }

        [Fact]
        public void Rotate()
        {
            var line = new Line(Vector3.Origin, new Vector3(5.0, 0.0));
            var rotated = line.Rotate(Vector3.Origin, 90.0);
            Assert.Equal(0.0, rotated.End.X, 10);
            Assert.Equal(5.0, rotated.End.Y, 10);
        }
    }
}
