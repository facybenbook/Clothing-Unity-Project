using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace IntelligentScissors
{

    public struct Boundary
    {
        public  int MIN_X, MAX_X, MIN_Y, MAX_Y;

        public int Width { get { return MAX_X - MIN_X; } }
        public int Height { get { return MAX_Y - MIN_Y; } }
    }

    #region 2D-ARRAY OF THE PICTURE
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
        public bool block;
    }
    #endregion

    public struct RGBPixelD
    {
        public double red, green, blue;
    }

    #region PAIR OF DOUBLE
    /// <summary>
    /// Holds the edge energy between 
    ///     1. a pixel and its right one (X)
    ///     2. a pixel and its bottom one (Y)
    /// </summary>
    public class Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Vector2D ( double x, double y )
        {
            X = x; Y = y;
        }
    }
    #endregion
    public class Point
    {
        public int X;
        public int Y;

        public Point(int x, int y )
        {
            X = x;
            Y = y;
        }
    }
    
    public class Edge : IWeightable
    {
        public int From, To;
        public double Weight { set; get; }
        public Edge ( int From, int To, double Weight )
        {
            this.From = From;
            this.To = To;
            this.Weight = Weight;
        }
    }

    public interface IWeightable
    {
        double Weight { set; get; }
    }
}
