using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Extensions
{
    public static class MeshExtensions
    {

        public static Vector3 ConvertUVCoordinateToLocalPosition (this Mesh mesh, Vector2 uv )
        {
            var tris = mesh.triangles;
            var uvs = mesh.uv;
            var verts = mesh.vertices;

            for ( int i = 0; i < tris.Length; i += 3 )
            {
                var u0 = uvs[tris[i]];
                var u1 = uvs[tris[i+1]];
                var u2 = uvs[tris[i+2]];
                var a = Area(u0, u1, u2);
                var a0 = Area( u1, u2, uv ) / a; if ( a0 < 0 ) continue;
                var a1 = Area( u2, u0, uv) / a; if ( a1 < 0 ) continue;
                var a2 = Area( u0, u1, uv) / a; if ( a2 < 0 ) continue;

                return a0 * verts [ tris [ i ] ] + a1 * verts [ tris [ i + 1 ] ] + a2 * verts [ tris [ i + 2 ] ];

            }

            return Vector3.zero;
        }

        private static float Area(Vector2 p1, Vector2 p2, Vector2 p3 )
        {
            Vector2 v1 = p1 - p3;
            Vector2 v2 = p2 - p3;
            return ( v1.x * v2.y - v1.y * v2.x ) * 0.5f;
        }
    }
}
