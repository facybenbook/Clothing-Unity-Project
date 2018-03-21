using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IntelligentScissors
{
    using System.Linq;
    public class Algorithm
    {
        
        public static List<int> Dijkstra ( int Source, Color [ ] pixels, int w, int h )
        {
            const double oo = 10000000000000000000; // infity value

            int length = pixels.Length;

            List<double> Distance = new List<double>();
            Distance = Enumerable.Repeat ( oo, length ).ToList ( );

            List<int> Previous = new List<int>();
            Previous = Enumerable.Repeat ( -1, length ).ToList ( );

            PeriorityQueue<Edge> MinimumDistances = new PeriorityQueue<Edge>();
            MinimumDistances.Push ( new Edge ( -1, Source, 0 ) );

            while ( !MinimumDistances.IsEmpty ( ) )
            {
                // get the shortest path so far 
                Edge CurrentEdge = MinimumDistances.Top();
                MinimumDistances.Pop ( );
                // check if this SP is vaild (i didn't vist this node with a less cost)
                if ( CurrentEdge.Weight >= Distance [ CurrentEdge.To ] )
                    continue;
                // save the previous 
                Distance [ CurrentEdge.To ] = CurrentEdge.Weight;
                Previous [ CurrentEdge.To ] = CurrentEdge.From;
                // Relaxing 
                List<Edge> neibours = Get_neighbours(CurrentEdge.To, pixels, w, h);
                for ( int i = 0; i < neibours.Count; i++ )
                {
                    Edge HeldEdge = neibours[i];
                    // if the relaxed path cost of a neighbour node is less than  it's previous one
                    if ( Distance [ HeldEdge.To ] > Distance [ HeldEdge.From ] + HeldEdge.Weight )
                    {
                        // set the relaxed cost to Distance  && pash it to the PQ
                        HeldEdge.Weight = Distance [ HeldEdge.From ] + HeldEdge.Weight;
                        MinimumDistances.Push ( HeldEdge );
                    }
                }
            }
            return Previous;
        }

        public static List<int> Backtracking (List<int> previous_list, int Dest )
        {
           List<int> track = new List<int>();
            track.Add ( Dest );
            int previous = previous_list[Dest];
            while ( previous != -1 )
            {
                track.Add ( previous );
                previous = previous_list [ previous ];
            }
            track.Reverse ( );
            return track;
        } 

        public static List<int> Dijkstra ( int Source, int Dest, Color[] pixels, int w, int h )
        {
            const double oo =  10000000000000000000;

            int length = pixels.Length;

            List<double> Distance = new List<double>();
            Distance = Enumerable.Repeat ( oo, length ).ToList();

            List<int> Previous = new List<int>();
            Previous = Enumerable.Repeat ( -1, length ).ToList ( );

            PeriorityQueue<Edge> MinimumDistances = new PeriorityQueue<Edge>();
            MinimumDistances.Push ( new Edge ( -1, Source, 0 ) );

            while ( !MinimumDistances.IsEmpty ( ) )
            {
                Edge CurrentEdge = MinimumDistances.Top();
                MinimumDistances.Pop ( );

                if ( CurrentEdge.Weight >= Distance [ CurrentEdge.To ] )
                    continue;

                Previous [ CurrentEdge.To ] = CurrentEdge.From;
                Distance [ CurrentEdge.To ] = CurrentEdge.Weight;

                if ( CurrentEdge.To == Dest ) break;

                List<Edge> neibours = Get_neighbours(CurrentEdge.To, pixels, w, h);
                for(int i = 0; i<neibours.Count; i++ )
                {
                    Edge heldEdge = neibours[i];
                    if(Distance[heldEdge.To] > Distance[heldEdge.From] + heldEdge.Weight )
                    {
                        heldEdge.Weight = Distance [ heldEdge.From ] + heldEdge.Weight;
                        MinimumDistances.Push ( heldEdge );
                    }
                }
            }

            return Previous;
        }

        

        #region Neighbours
        public static List<Edge> Get_neighbours ( int index, Color[] pixels, int w, int h)//RGBPixel [ , ] ImageMatrix )
        {
            const float maxValue = 10000000000000000f;
            List<Edge> neighbours = new List<Edge>();

            int X = index % w;
            int Y = index / w;

            float cost = 0;
            // calculate the gradient with right and bottom neighbour
            var Gradient = CalculatePixelEnergies(index, pixels, w, h);// (X, Y, ImageMatrix);
            if ( X < w - 1 ) // have a right neighbour ?  
            {
                cost = Gradient.x == 0 ? maxValue : 1f / ( Gradient.x );
                neighbours.Add ( new Edge ( index, index + 1, cost ) );
            }

            if ( Y < h - 1 ) // have a Bottom neighbour ?
            {

                cost = Gradient.y == 0 ? maxValue : 1f / ( Gradient.y );
                neighbours.Add ( new Edge ( index, index + w, cost ) );
            }

            if ( Y > 0 ) // have a Top neighbour ?
            {
                // calculate the gradient with top neighbour
                //Gradient = ImageOperations.CalculatePixelEnergies ( X, Y - 1, ImageMatrix );
                Gradient = CalculatePixelEnergies(index - w, pixels, w, h);
                cost = Gradient.y == 0 ? maxValue : 1f / ( Gradient.y );
                neighbours.Add ( new Edge ( index, index - w, cost ) );
                
            }

            if ( X > 0 ) // have a Left neighbour ?
            {
                // calculate the gradient with left neighbour
                //Gradient = ImageOperations.CalculatePixelEnergies ( X - 1, Y, ImageMatrix );
                Gradient = CalculatePixelEnergies ( index - 1, pixels, w, h );
                cost = Gradient.x == 0 ? maxValue : 1f / ( Gradient.x );
                neighbours.Add ( new Edge ( index, index - 1, cost ) );
                
            }
            return neighbours; // return neighbours
        }
        #endregion

        #region CalculatePixelEnergies
        /// <summary>
        /// Calculate edge energy between
        ///     1. the given pixel and its right one (X)
        ///     2. the given pixel and its bottom one (Y)
        /// </summary>
        /// <param name="x">pixel x-coordinate</param>
        /// <param name="y">pixel y-coordinate</param>
        /// <param name="ImageMatrix">colored image matrix</param>
        /// <returns>edge energy with the right pixel (X) and with the bottom pixel (Y)</returns>
        public static Vector2 CalculatePixelEnergies (int index, Color [ ] pixels, int w, int h )// )//( int x, int y, RGBPixel [ , ] ImageMatrix )
        {
            if ( pixels == null || pixels.Length == 0)// ImageMatrix == null )
                Debug.LogError ( "image is not set!" );
            //throw new Exception ( "image is not set!" );
            Vector2 gradient = CalculateGradientAtPixel (index, pixels, w, h );//(x, y, ImageMatrix);
            float gradientMagnitude = Mathf.Sqrt(gradient.x * gradient.x  + gradient.y * gradient.y);
            float edgeAngle = Mathf.Atan2(gradient.y, gradient.x);
            float rotatedEdgeAngle = edgeAngle + Mathf.PI / 2.0f;
            Vector2 energy = new Vector2(
             Mathf.Abs(gradientMagnitude * Mathf.Cos(rotatedEdgeAngle)),
             Mathf.Abs(gradientMagnitude * Mathf.Sin(rotatedEdgeAngle)));
            return energy;
        }
        #endregion

        #region Private Function, CalculateGradientAtPixel
        /// <summary>
        /// Calculate Gradient vector between the given pixel and its right and bottom ones
        /// </summary>
        /// <param name="x">pixel x-coordinate</param>
        /// <param name="y">pixel y-coordinate</param>
        /// <param name="ImageMatrix">colored image matrix</param>
        /// <returns></returns>
        private static Vector2 CalculateGradientAtPixel (int index, Color [ ] pixels, int w, int h )//( int x, int y,  RGBPixel [ , ] ImageMatrix )
        {
            Vector2 gradient = new Vector2(0, 0);
            //RGBPixel mainPixel = ImageMatrix[y, x];
            Color pixel = pixels[index];
            //double pixelGrayVal = 0.21 * mainPixel.red + 0.72 * mainPixel.green + 0.07 * mainPixel.blue;
            float pixelGrayVal = 0.21f * pixel.r + 0.72f * pixel.g + 0.07f * pixel.b;
            int x = index % w;
            int y = index / w;
            if ( y == h - 1)//y == GetHeight ( ImageMatrix ) - 1 )
            {
                //boundary pixel.
                //for (int i = 0; i < 3; i++)
                //{
                gradient.y = 0;
                //}
            }
            else
            {
                //RGBPixel downPixel = ImageMatrix[y + 1, x];
                Color downPixel = pixels[index + w];
                //double downPixelGrayVal = 0.21 * downPixel.red + 0.72 * downPixel.green + 0.07 * downPixel.blue;
                float downPixelGrayVal = 0.21f * downPixel.r + 0.72f * downPixel.g + 0.07f * downPixel.b;

                gradient.y = pixelGrayVal - downPixelGrayVal;
            }
            if ( x == w - 1)// x == GetWidth ( ImageMatrix ) - 1 )
            {
                //boundary pixel.
                gradient.x = 0;
            }
            else
            {
                //RGBPixel rightPixel = ImageMatrix[y, x + 1];
                Color rightPixel = pixels[index + 1];
                float rightPixelGrayVal = 0.21f * rightPixel.r + 0.72f * rightPixel.g + 0.07f * rightPixel.b;
                gradient.x = pixelGrayVal - rightPixelGrayVal;
            }
            return gradient;
        }
        #endregion       
    }
}
