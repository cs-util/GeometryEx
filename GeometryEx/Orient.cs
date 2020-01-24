using System;
using System.Collections.Generic;
using System.Text;

namespace GeometryEx
{
    /// <summary>
    /// A list of compass orientations used to designate locations on a 2D box.
    /// N, S, E, and W define middle points on each orthogonal side of the box.
    /// NE, NW, SE, and SW correspond to the corners of the box.
    /// C corresponds to the center of the box.
    /// Other compass points define locations along the relevant side between the cardinal and corner points.
    /// See documentation of corresponding properties of the CompassBox class for full documentation.
    /// </summary>
    public enum Orient { C, N, NNE, NE, ENE, E, ESE, SE, SSE, S, SSW, SW, WSW, W, WNW, NW, NNW };

    /// <summary>
    /// A list of compass orientations used to designate locations on a 2D box.
    /// N, S, E, and W define middle points on each orthogonal side of the box.
    /// See documentation of corresponding properties of the CompassBox class for full documentation.
    /// </summary>   
    public enum Cardinal { N = 1, E = 5, S = 9, W = 13 };

    /// <summary>
    /// A list of box corners as compass designations.
    /// NE = maximum X and Y corner.
    /// SE = maximum X and minimum Y corner.
    /// SW = minimum X and Y corner.
    /// NW = minimum X and maximum Y corner.
    /// </summary>
    public enum Corner { NE = 3, SE = 7, SW = 11, NW = 15 };

    /// <summary>
    /// A list of box corners as compass designations.
    /// CenterSpan = Span the center of the bounding box on X and Y axes.
    /// CenterX = Pass a grid line through the center of the bounding box on the X axis.
    /// CenterY = Pass a grid line through the center of the bounding box on the Y axis.
    /// CenterXY = Pass a grid line through the center of the bounding box on both the X and Y axes.
    /// MaxX = Pass a grid line through the maximum X axis of the bounding box and CenterSpan Y axis grid lines.
    /// MaxY = Pass a grid line through the maximum Y axis of the bounding box and CenterSpan X axis grid lines.
    /// MaxXY = Pass X and Y grid lines through the maximum axes of the bounding box.
    /// MinX = Pass a grid line through the minimum X axis of the bounding box and CenterSpan Y axis grid lines.
    /// MinY = Pass a grid line through the minimum Y axis of the bounding box and CenterSpan X axis grid lines.
    /// MinXY = Pass X and Y grid lines through the minimum axes of the bounding box. 
    /// </summary>
    public enum GridPosition { CenterSpan, CenterX, CenterY, CenterXY, MaxX, MaxY, MaxXY, MinX, MinY, MinXY };
}
