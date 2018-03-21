using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TriangleNet.Extensions
{
    using Geometry;
    using Topology;

    using System.Linq;

    public static class TriangleExtensions
    {

        public static Vector2 Vector2 (this Vertex vertex )
        {
            return new Vector2 ( ( float ) vertex.x, ( float ) vertex.y );
        }

        #region Area
        public static float Area(this Triangle triangle )
        {
            return Area ( triangle.vertices );
        }
        public static float Area ( this IEnumerable<Point> poly )
        {
            double area = 0f;

            Point last = poly.Last();
            for ( int i = 0, j = poly.Count ( ) - 1; i < poly.Count ( ); j = i++ )
            {
                var pi = poly.ElementAt(i);
                var pj = poly.ElementAt(j);
                var xi = pi.X;
                var yi = pi.Y;
                var xj = pj.X;
                var yj =  pj.Y;

                var temp =  (xi * yj - xj * yi) / 2;
                area += temp;
            }

            return Mathf.Abs ( ( float ) area );
        }
        #endregion Area

        #region Mass Center
        public static Point MassCenter(this Triangle triangle )
        {
            return MassCenter(triangle.vertices);
        }

        public static Point MassCenter(this IEnumerable<Point> poly )
        {
            double area = 0f;
            double cx = 0f;
            double cy = 0f;

            Point last = poly.Last();
            for ( int i = 0, j = poly.Count() - 1; i < poly.Count ( ); j = i++ )
            {
                var pi = poly.ElementAt(i);
                var pj = poly.ElementAt(j);
                var xi = pi.X;
                var yi = pi.Y;
                var xj = pj.X;
                var yj =  pj.Y;

                var temp =  xi * yj - xj * yi;
                area += temp;
                cx += ( xi + xj ) * temp;
                cy += ( yi + yj ) * temp;
            }
            
            if ( Mathf.Abs ( ( float ) area ) < 1e-7 )
            {
                return new Point ( );
            }
            area *= 3;
            return new Point ( cx / area, cy / area );
        }
        #endregion Mass  Center

    }
}
