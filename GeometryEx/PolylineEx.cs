using System;
using System.Linq;
using System.Collections.Generic;
using ClipperLib;
using Elements.Geometry;

namespace GeometryEx
{
    public static class PolylineEx
    {
        /// <summary>
        /// Reduces Polyline vertices.
        /// </summary>
        /// <param name="tolerance">The tolerated deviation to include a vertex.</param>
        /// <returns>A new Polyline.</returns>
        public static Polyline Straighten(this Polyline polyline, double tolerance)
        {
            return new Polyline(SimplifyNet.Straighten(polyline.Vertices.ToList(), tolerance));
        }

        /// <summary>
        /// Reduces Polyline vertices.
        /// </summary>
        /// <param name="tolerance">The tolerated deviation to include a vertex.</param>
        /// <returns>A new Polyline.</returns>
        public static Polyline Simplify(this Polyline polyline, double minLength)
        {
            return new Polyline(Shaper.Simplify(polyline.Vertices.ToList(), minLength));
        }
    }
}
