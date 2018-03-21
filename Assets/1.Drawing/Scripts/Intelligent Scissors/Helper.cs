using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace IntelligentScissors
{
    public static class Helper
    {

        public static double Distance ( int N1, int N2, int Width )
        {
            Vector2D P1 = Helper.Unflatten(N1, Width);
            Vector2D P2 = Helper.Unflatten(N2, Width);
            return Math.Sqrt ( Math.Pow ( P1.X - P2.X, 2 ) + Math.Pow ( P1.Y - P2.Y, 2 ) );
        }

        public static bool Vaild_Pixel ( int X, int Y, RGBPixel [ , ] ImageMatrix )
        {
            bool Vaild_X = (X >= 0 && X < ImageOperations.GetWidth(ImageMatrix));
            bool Vaild_Y = (Y >= 0 && Y < ImageOperations.GetHeight(ImageMatrix));
            return Vaild_X && Vaild_Y;
        }

        public static RGBPixel [ , ] COPY ( RGBPixel [ , ] ImageMatrix )
        {
            int Width = ImageOperations.GetWidth(ImageMatrix);
            int Height = ImageOperations.GetHeight(ImageMatrix);


            RGBPixel[,] selected_image = new RGBPixel[Height, Width];
            for ( int r = 0; r < Height; r++ )
                for ( int c = 0; c < Width; c++ )
                    selected_image [ r, c ] = ImageMatrix [ r, c ];

            return selected_image;
        }

        public static RGBPixel [ , ] COPY_Segment ( RGBPixel [ , ] ImageMatrix, Boundary bondry )
        {
            // copy a segment  from image matrix into anew one
            int Width = bondry.MAX_X - bondry.MIN_X; // new segment widtrh 
            int Height = bondry.MAX_Y - bondry.MIN_Y; // new segment height


            RGBPixel[,] selected_image = new RGBPixel[Height + 1, Width + 1];
            for ( int r = 0; r <= Height; r++ )
                for ( int c = 0; c <= Width; c++ )
                    selected_image [ r, c ] = ImageMatrix [ bondry.MIN_Y + r, bondry.MIN_X + c ];

            return selected_image;
        }

        public static bool IN_Boundary ( int Target, Boundary bondry, int Width )
        {
            Vector2D Target2d = Helper.Unflatten(Target, Width);
            bool Vaild_X = (Target2d.X >= bondry.MIN_X && Target2d.X < bondry.MAX_X);
            bool Vaild_Y = (Target2d.Y >= bondry.MIN_Y && Target2d.Y < bondry.MAX_Y);

            return Vaild_X && Vaild_Y;
        }

        #region crosspond
        //take point in a 2d plane and retrun the crossponding in(the small segment) boundry plane 
        public static Point crosspond ( Point P, Boundary bondry )
        {
            P.X = P.X + bondry.MIN_X;
            P.Y = P.Y + bondry.MIN_Y;
            return P;
        }
        public static List<Point> crosspond ( List<Point> Path, Boundary bondry )
        {
            for ( int i = 0; i < Path.Count; i++ )
                Path [ i ] = Helper.crosspond ( Path [ i ], bondry );
            return Path;
        }
        public static int crosspond ( int node_number, Boundary bondry, int main_Width, int segment_Width )
        {
            Vector2D node2d = Helper.Unflatten(node_number, main_Width);
            node2d.X = node2d.X - bondry.MIN_X;
            node2d.Y = node2d.Y - bondry.MIN_Y;
            int newnode = Helper.Flatten((int)node2d.X, (int)node2d.Y, segment_Width);
            return newnode;
        }
        #endregion

        #region GO THROUGH 1D TO 2D AND VICE VERSA

        /// <summary>
        /// convert 2d index to 1d index  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <returns>node number in flatten 1d array</returns>
        public static int Flatten ( int X, int Y, int width )
        {
            return ( X ) + ( Y * width );
        }

        /// <summary>
        ///convert 1d index to 2d index 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="width"></param>
        /// <returns> vector2d  (X,Y) </X></returns>
        public static Vector2D Unflatten ( int Index, int width )
        {
            // y -> row ,  x -> column  
            return new Vector2D ( ( int ) Index % ( int ) width, ( int ) Index / width );
        }
        #endregion

        #region ASK TO DELETE
        public static List<T> AppendToList<T> ( List<T> dest, List<T> sourc )
        {
            if ( dest == null || sourc == null )
            {
                throw new ArgumentNullException ( );
            }
            List<T> tmp = dest;
            for ( int i = 0; i < sourc.Count; i++ )
            {
                tmp.Add ( sourc [ i ] );
            }
            return tmp;
        }
        #endregion

        public static List<T> AppendToList<T> ( List<T> dest, T [ ] sourc )
        {
            if ( dest == null || sourc == null )
            {
                return null;
                throw new ArgumentNullException ( );
            }
            List<T> tmp = dest;
            for ( int i = 0; i < sourc.Length; i++ )
            {
                tmp.Add ( sourc [ i ] );
            }
            return tmp;
        }
    }
}


