

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ClipperLib;
using System.Linq;

using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;


using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Smoothing;

using UnityEngine.EventSystems;

namespace Clothing.Polygon
{

    [RequireComponent ( typeof ( MeshFilter ), typeof ( MeshRenderer ), typeof ( MeshCollider ) )]
    public class LineMeshBuilder : MonoBehaviour
    {

        public float width = 0.1F;

        public Vector3 startPosition { set; get; }
        public Vector3 endPosition { set; get; }

        public bool IsCurve
        {
            get
            {
                 return CGAlgorithm.Distance2D_PointToSegment(curvePosition, startPosition, endPosition) > 0.001f;
            }
        }
        public Vector3 curvePosition { set; get; }
        //public Vector3 centerPosition { set; get; }

        [Range(0.01f, 1)]
        public float curveDetail = 0.1F;

        //private Vector3 centerPosition { set; get; }

        private Vector3 [ ] Vertices { set; get; }

        private Vector2 [ ] UV { set; get; }

        private int [ ] Triangles { set; get; }

        private Vector3 [ ] Normals { set; get; }

        private Vector4 [ ] Tangents { set; get; }

        void HandleOnUpdateVertices ( )
        {
            //HandleOnUpdateCurve ( );
            if ( !IsCurve )
            {
                HandleOnUpdateSegment ( );
            }
            else
            {
                HandleOnUpdateCurve ( );
            }
        }

        void HandleOnUpdateCurve ( )
        {
            var count = Mathf.Ceil(1 / curveDetail);

            //Build the curve
            var handler = Bezier.CalcHandler(startPosition, endPosition, curvePosition); ;
            //var handler = curvePosition;// //Bezier.Control(startPosition, endPosition, curvePosition);
            var center = Bezier.Square ( startPosition, endPosition, handler, 0.5F );
            //centerPosition = new Vector3 ( center.x, center.y, 0 );

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uv = new List<Vector2>();
            var normals = new List<Vector3>();
            var tangents = new List<Vector4>();

            float UVX = 0F;

            Vector2 I0, I1, I2, I3;

            for ( int i = 0; i < count; i++ )
            {
                var pre = Bezier.Square ( startPosition,  endPosition,  handler, (float) ( i - 1 ) / count );
                var cur = Bezier.Square ( startPosition,  endPosition, handler,  (float) i / count );
                var next = Bezier.Square ( startPosition,  endPosition, handler,  (float) ( i + 1 ) / count );

                var preTegent = cur - pre;
                var nextTegent = next - cur;
                var normal = Vector3.back;
                var preBinormal = Vector3.zero;
                var nextBinormal = Vector3.zero;

                Vector3.OrthoNormalize ( ref normal, ref preTegent, ref preBinormal );
                Vector3.OrthoNormalize ( ref normal, ref nextTegent, ref nextBinormal );

                var preOffset = width * 0.5F * preBinormal;
                var nextOffset = width * 0.5F * nextBinormal;

                var p0 = pre + preOffset;// (Vector2)(pre + preOffset);
                var p1 = cur + preOffset;// (Vector2) (cur + preOffset);

                var p2 = cur + nextOffset;// (Vector2) (cur + nextOffset);
                var p3 = next + nextOffset;// (Vector2) (next + nextOffset);

                var p4 =pre - preOffset;// (Vector2) (pre - preOffset);
                var p5 =cur - preOffset;// (Vector2) (cur - preOffset);

                var p6 = cur - nextOffset;// (Vector2) (cur - nextOffset);
                var p7 = next - nextOffset;// (Vector2)  (next - nextOffset);

                var preLength = Vector3.Distance(cur, pre);
                var nextLength = Vector3.Distance(next, cur);

                int size = vertices.Count;

                if ( size != 0 && CGAlgorithm.Intersect2D_2Segments ( p0, p1, p2, p3, out I0, out I1 ) > 0 )
                {
                    int interIndex = size - 2;
                    vertices [ interIndex ] = I0;
                    vertices.AddRange ( new Vector3 [ ] { p6, I0, p3, p7 } );

                    triangles.AddRange ( new int [ ] { size, size + 2, size + 1, size, size + 3, size + 2, size - 1, size, interIndex } );

                }
                else if ( size != 0 && CGAlgorithm.Intersect2D_2Segments ( p4, p5, p6, p7, out I2, out I3 ) > 0 )
                {
                    int interIndex = size - 1;
                    vertices [ interIndex ] = I2;
                    vertices.AddRange ( new Vector3 [ ] { I2, p2, p3, p7 } );
                    triangles.AddRange ( new int [ ] { size, size + 2, size + 1, size, size + 3, size + 2, interIndex, size + 1, size - 2 } );

                }
                else
                {
                    vertices.AddRange ( new Vector3 [ ] { p6, p2, p3, p7 } );
                    triangles.AddRange ( new int [ ] { size, size + 2, size + 1, size, size + 3, size + 2 } );

                }

                var preUVX = UVX;
                UVX += nextLength / width;
                uv.AddRange ( new Vector2 [ ] { new Vector2 ( preUVX, 0 ), new Vector2 ( preUVX, 1 ), new Vector2 ( UVX, 1 ), new Vector2 ( UVX, 0 ) } );
                normals.AddRange ( new Vector3 [ ] { Vector3.back, Vector3.back, Vector3.back, Vector3.back } );
                tangents.AddRange ( new Vector4 [ ] { new Vector4 ( -1, 0, 0, -1 ), new Vector4 ( -1, 0, 0, -1 ), new Vector4 ( -1, 0, 0, -1 ), new Vector4 ( -1, 0, 0, -1 ) } );
            }

            Vertices = vertices.ToArray ( );
            Triangles = triangles.ToArray ( );
            UV = uv.ToArray ( );
            Normals = normals.ToArray ( );
            Tangents = tangents.ToArray ( );

        }

        void HandleOnUpdateSegment ( )
        {

            curvePosition = ( startPosition + endPosition ) * 0.5F;
            //centerPosition = curvePosition;

            var tegent = endPosition - startPosition;
            var normal = Vector3.back;
            var binormal = Vector3.zero;

            Vector3.OrthoNormalize ( ref normal, ref tegent, ref binormal );

            var offset = width * 0.5F * binormal;
            Vertices = new Vector3 [ ] { startPosition - offset, startPosition + offset, endPosition + offset, endPosition - offset };
            Triangles = new int [ ] { 0, 2, 1, 0, 3, 2 };
            Normals = new Vector3 [ ] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
            Tangents = new Vector4 [ ] { new Vector4 ( -1, 0, 0, -1 ), new Vector4 ( -1, 0, 0, -1 ), new Vector4 ( -1, 0, 0, -1 ), new Vector4 ( -1, 0, 0, -1 ) };

            var UVScale = Vector3.Distance(endPosition, startPosition) / width;
            UV = new Vector2 [ ] { new Vector2 ( 0, 0 ), new Vector2 ( 0, 1 ), new Vector2 ( UVScale, 1 ), new Vector2 ( UVScale, 0 ) };

        }

        public void HandleOnUpdateMesh ( )
        {

            if ( Vertices.Length < 3 )
                throw new System.Exception ( "A Mesh must needs more than 3 points, no less!" );

            //var _mesh = GetComponent<MeshFilter>().mesh;
            //if ( !_mesh )
            //{
            //    _mesh = new UnityEngine.Mesh ( );
            //    GetComponent<MeshFilter> ( ).mesh = _mesh;
            //}
            var _mesh = new UnityEngine.Mesh();
            _mesh.Clear ( );
            _mesh.vertices = Vertices;
            _mesh.uv = UV;
            _mesh.triangles = Triangles;
            _mesh.normals = Normals;
            GetComponent<MeshFilter> ( ).mesh = _mesh;
            GetComponent<MeshCollider> ( ).sharedMesh = _mesh;
        }

        public void OnRender ( )
        {
            HandleOnUpdateVertices ( );
            HandleOnUpdateMesh ( );
            //HandleOnUpdateCollider ( );
        }


    }
}
