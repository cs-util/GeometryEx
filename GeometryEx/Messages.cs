using System;
using System.Collections.Generic;
using System.Text;

namespace GeometryEx
{
    /// <summary>
    /// Common exception messages.
    /// </summary>
    public static class Messages
    {
        public const string NEGATIVE_VALUE_EXCEPTION = "Value must be zero or greater.";

        public const string POLYGON_SHAPE_EXCEPTION = "Values will result in an unexpected shape. Examine polygon relationships and dimensions.";

        public const string POLYGON_NULL_EXCEPTION = "Null value instead of Polygon.";

        public const string NON_POSITIVE_VALUE_EXCEPTION = "Value must be greater than zero.";

        public const string ZERO_VALUE_EXCEPTION = "Value must not be zero.";
    }
}
