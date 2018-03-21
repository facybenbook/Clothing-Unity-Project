using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon

{

    public static class Bezier
    {

        public static float Cubic ( float form, float h0, float h1, float to, float t )
        {
            float u = 1f-t;
            float uu = u*u;
            float tt = t*t;
            float uuu = uu * u;
            float ttt = tt*t;
            return uuu * form + 3 * uu * t * h0 + 3 * u * ttt * h1 + ttt * to;

        }

        public static Vector3 Cubic ( Vector3 form, Vector3 h0, Vector3 h1, Vector3 to, float t )
        {
            float x = Cubic(form.x, h0.x, h1.x, to.x, t);
            float y = Cubic(form.y, h0.y, h1.y, to.y, t);
            float z = Cubic(form.z, h0.z, h1.z, to.z, t);
            return new Vector3 ( x, y, z );
        }


        public static float Square ( float from, float to, float handler, float t )
        {
            return from * ( 1 - t ) * ( 1 - t ) + handler * 2 * ( 1 - t ) * t + to * t * t;
        }

        public static Vector3 Square ( Vector3 from, Vector3 to, Vector3 handler, float t )
        {
            from.x = Square ( from.x, to.x, handler.x, t );
            from.y = Square ( from.y, to.y, handler.y, t );
            from.z = Square ( from.z, to.z, handler.z, t );
            return from;
        }

        public static Vector3 CalcHandler ( Vector3 from, Vector3 to, Vector3 curve )
        {
            //var center = Vector3.Lerp(from, to, 0.5f);
            //return center + (curve - center) * 2;
            var axis = Vector3.Normalize(to - from);
            var dot = Vector3.Dot(axis, curve - from);
            var linePoint = from + axis * dot;
            return linePoint + ( curve - linePoint ) * 2;
        }

        public static Vector3 CalcCurve ( Vector3 from, Vector3 to, Vector3 handler )
        {
            var axis = Vector3.Normalize(to - from);
            var dot = Vector3.Dot(axis, handler - from);
            var linePoint = from + axis *dot;
            return linePoint + ( handler - linePoint ) * 0.5F;

        }

        public static float BinaryFactor ( Vector3 from, Vector3 to, Vector3 curve, Vector3 pos )
        {
            var handler = CalcHandler(from, to, curve);


            int count = 0;
            float half = 0.5f;
            float factor = 0.5f;
            while ( count < 100 )
            {

                half *= 0.5f;
                var first = Square(from, to, handler, factor - half);
                var last = Square(from,  to,handler, factor + half);

                var fd = Vector3.Distance(first, pos);
                var ld = Vector3.Distance(last, pos);

                if ( Mathf.Abs ( fd - ld ) < 0.00001f )//fd == lf
                    break;

                if ( fd > ld )
                {
                    factor += half;
                }
                else if ( fd < ld )
                {
                    factor -= half;
                }
                count++;
            }

            return factor;

        }

        public static float ArcLength ( Vector3 from, Vector3 to, Vector3 curve )
        {
            var handler = CalcHandler(from, to, curve);

            Vector2 v;
            Vector2 w;
            v.x = 2 * ( handler.x - from.x );
            v.y = 2 * ( handler.y - from.y );
            w.x = to.x - 2 * handler.x + from.x;
            w.y = to.y - 2 * handler.y + from.y;

            float uu = 4 * ( w.x * w.x + w.y * w.y );

            if ( uu < 0.00001f )
            {
                return ( float ) Mathf.Sqrt ( ( to.x - from.x ) * ( to.x - from.x ) + ( to.y - from.y ) * ( to.y - from.y ) );
            }

            float vv = 4 * ( v.x * w.x + v.y * w.y );
            float ww = v.x * v.x + v.y * v.y;

            float t1 = ( float ) ( 2 * Mathf.Sqrt ( uu * ( uu + vv + ww ) ) );
            float t2 = 2 * uu + vv;
            float t3 = vv * vv - 4 * uu * ww;
            float t4 = ( float ) ( 2 * Mathf.Sqrt ( uu * ww ) );

            return ( float ) ( ( t1 * t2 - t3 * Mathf.Log ( t2 + t1 ) - ( vv * t4 - t3 * Mathf.Log ( vv + t4 ) ) ) / ( 8 * Mathf.Pow ( uu, 1.5f ) ) );
        }

        public static List<Vector3> Split (Vector3 from, Vector3 to, Vector3 curve, int count )
        {

            List<Vector3> segments = new List<Vector3>();
            segments.Add ( from );

            if ( count <= 0 ) return segments;
            var handler = Bezier.CalcHandler(from, to,  curve);

            for ( int i = 1; i <= count; i++ )
            {
                var t = (float)i / count;
                segments.Add(Bezier.Square ( from, to, handler, t ) );
            }
            return segments;
        }

    }
}
