using System.Collections.Generic;
using Xunit;
using Elements.Geometry;
using GeometryEx;


namespace GeometryExTests
{
    public class VectorExTests
    {
        [Fact]
        public void FarthestFrom()
        {
            var point = Vector3.Origin;
            var points = new List<Vector3>();
            points.Add(new Vector3(5.0, 5.0));
            points.Add(new Vector3(6.0, 6.0));
            points.Add(new Vector3(7.0, 7.0));
            points.Add(new Vector3(8.0, 8.0));

            var farPoint = point.FarthestFrom(points);
            Assert.Equal(8.0, farPoint.X);
            Assert.Equal(8.0, farPoint.Y);
        }

        [Fact]
        public void IsListed()
        {
            var points = new List<Vector3>
            {
                new Vector3(5.0, 5.0),
                new Vector3(6.0, 6.0),
                new Vector3(7.0, 7.0),
                new Vector3(8.0, 8.0)
            };
            Assert.True(new Vector3(5.0, 5.0).IsListed(points));
            Assert.False(Vector3.Origin.IsListed(points));
        }

        [Fact]
        public void MoveFromTo()
        {
            var point = new Vector3(5.0, 5.0, 5.0);
            point = point.MoveFromTo(Vector3.Origin, new Vector3(5.0, 5.0, 5.0));

            Assert.Equal(10.0, point.X);
            Assert.Equal(10.0, point.Y);
            Assert.Equal(10.0, point.Z);
        }

        [Fact]
        public void NearestTo()
        {
            var point = Vector3.Origin;
            var points = new List<Vector3>();
            points.Add(new Vector3(5.0, 5.0));
            points.Add(new Vector3(6.0, 6.0));
            points.Add(new Vector3(7.0, 7.0));
            points.Add(new Vector3(8.0, 8.0));

            var nearPoint = point.NearestTo(points);
            Assert.Equal(5.0, nearPoint.X);
            Assert.Equal(5.0, nearPoint.Y);
        }

        [Fact]
        public void Rotate()
        {
            var point = new Vector3(5.0, 5.0);
            var rotated = point.Rotate(Vector3.Origin, 180.0);
            Assert.Equal(-5.0, rotated.X, 10);
            Assert.Equal(-5.0, rotated.Y, 10);
        }
    }
}