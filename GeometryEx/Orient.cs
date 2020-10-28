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
    /// Defines four Cartesian coordinate quadrants.
    /// </summary>
    public enum Quadrant { I, II, III, IV };

    //public enum Slope { Concave = -1, Flat = 0, Convex = 1 };
}
