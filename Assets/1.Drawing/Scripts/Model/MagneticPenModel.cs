using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{

    using strange.extensions.context.api;
    using Polygon;
    using IntelligentScissors;

    using System.Linq;

    using Utils.Extensions;


    public class MagneticPenModel : BasePenModel
    {


        int Width;
        int Height;

        Color[] Caches;

        private Texture2D _photo;
        public Texture2D Photo
        {
            get
            {

                if ( _photo == null )
                {

                    _photo = Resources.Load<Texture2D> ( "67" );
                    int miplevel =1;
                    int n = 1 << miplevel;
                    Width = _photo.width / n;
                    Height = _photo.height / n;
                    Caches = _photo.GetPixels ( miplevel );
                }
                return _photo;
            }
        }

        Vector3 lastPosition;

        Vector2 srcUV;

        List<int> Mainselection = new List<int>();
        Transform target;
        _GLUtils glUtils;

        public override void DrawStart ( Vector3 position )
        {
            Vector2 uv;
            Transform trans;
            if ( !RaycastHitToUVCoordinate ( position, out uv, out trans ) )
                return;

            if ( Photo == null ) return;

            OnInit ( );

            CreatePoint ( position );

            lastPosition = position;

            srcUV = uv;
            target = trans;
            glUtils = target.GetComponent<_GLUtils> ( );
            glUtils.Reset ( );

        }

        public override void DrawTo ( Vector3 position )
        {
            if ( Vector3.Distance ( lastPosition, position ) < 0.5f )
                return;

            Vector2 uv;
            Transform trans;

            if ( !RaycastHitToUVCoordinate ( position, out uv, out trans ) )
                return;

            var srcIndex = CalcIndex(srcUV, Width, Height);
            var destIndex = CalcIndex(uv, Width, Height);

            var allPaths = Algorithm.Dijkstra ( srcIndex, destIndex, Caches, Width, Height );

            List<int> path = Algorithm.Backtracking ( allPaths, destIndex);

            //Mainselection.AddRange ( path );
            //Debug.Log ("Main Count : " + Mainselection.Count +  ", Path Size : " + path.Count );
            int mergeIndex = Merge ( Mainselection, path );

            #region Draw Point and Line
            if ( path.Count < 2 ) return;

            var middle = path[path.Count / 2];
            var midPosition = ConvertIndexToWorldPosition(middle, Width, Height, target);
            var mergePosition = ConvertIndexToWorldPosition(mergeIndex, Width, Height, target);

            int size = pathEntity.Points.Count;
            pathEntity.Points [ size - 1 ].position = mergePosition.Value;
            pathEntity.Points [ size - 1 ].curvePosition = midPosition.Value;


            CreatePoint ( position );

            CreateLine ( size - 1, size );

            #endregion

            var points = Mainselection.
                Select(i => ConvertIndexToWorldPosition(i, Width, Height, target).Value).
                Select ( p => Camera.main.WorldToScreenPoint ( p ) ).
                Select ( p => new Vector3 ( p.x, p.y, 0.5f ) ).
                ToList();
            glUtils.Paths.Add( points);

            lastPosition = position;

            srcUV = uv;
        }

        public override void DrawEnd ( )
        {
            int size = pathEntity.Points.Count;

            if ( size < 3 )
            {
                GameObject.Destroy ( pathView.gameObject );
                pathView = null;
                return;
            }

            var first = pathEntity.Points[0];
            var last = pathEntity.Points[size - 1];
            last.curvePosition = ( first.position + last.position ) * 0.5f;

            CreateLine ( size - 1, 0 );

            lookup.libs.Add ( pathView, pathEntity );

            pathView = null;
            pathEntity = null;
        }
        int Merge ( List<int> mainLine, List<int> newLine )
        {
            int result = -1;
            if ( mainLine.Count == 0 )
            {
                mainLine.AddRange ( newLine );
                return mainLine.Last ( );
            }

            int mainSize = mainLine.Count;
            int newSize = newLine.Count;
            //int mainLast = mainSize - 1;
            //int newFirst = 0;
            int i, j;
            for ( i = 0, j = mainSize - 1; i < newSize; i++, j-- )
            {
                if ( j == 0 )
                    break;

                int newNode = newLine[i];
                int mainNode = mainLine[j];

                var distance = Mathf.Abs(newNode - mainNode) % Width;

                if ( distance > 4 )
                    break;
            }
            Debug.Log ( "New : " + i + ", Main : " + j );

            mainLine.RemoveRange ( j, mainSize - j );
            result = mainLine.Last ( );
            newLine.RemoveRange ( 0, i );
            mainLine.AddRange ( newLine );

            return result;
        }

        Vector3? ConvertIndexToWorldPosition ( int index, int w, int h, Transform transform )
        {
            var meshFilter = target.GetComponent<MeshFilter>();
            if ( !meshFilter ) return null;
            var mesh = meshFilter.sharedMesh;

            var x =(float) (index % w);
            var y = (float)( index / w);
            var u = x / w;
            var v = y / h;

            var localPos = mesh.ConvertUVCoordinateToLocalPosition(new Vector2(u, v));
            return transform.TransformPoint ( localPos );

        }

        private int CalcIndex ( Vector2 uv, int w, int h )
        {
            return ( int ) ( uv.x * w ) + ( int ) ( uv.y * h ) * w;// ( int)((1.0f - uv.y) * h) * w;
        }

        private bool RaycastHitToUVCoordinate ( Vector3 position, out Vector2 uv, out Transform trans )
        {
            uv = Vector2.zero;
            trans = null;
            var origin = new Vector3(position.x, position.y, -10);
            var layers = 1 << LayerMask.NameToLayer("Canvas");
            RaycastHit hit;
            if ( Physics.Raycast ( origin, Vector3.forward, out hit, Mathf.Infinity, layers ) )
            {
                trans = hit.transform;
                uv = hit.textureCoord;
                return true;
            }
            return false;

        }
    }



}