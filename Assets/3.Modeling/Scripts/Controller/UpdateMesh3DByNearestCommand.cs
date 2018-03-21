using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Modeling

{
    using TriangleNet.Meshing;
    using TriangleNet.Geometry;
    using TriangleNet.Tools;
    using TriangleNet.IO;
    using TriangleNet.Smoothing;
    using TriangleNet.Voronoi;
    using TriangleNet.Topology;//.DCEL;
    using System.Linq;

    using TriangleNet.Extensions;

    using strange.extensions.context.api;
    using strange.extensions.command.impl;
    using strange.extensions.dispatcher.eventdispatcher.impl;

    using Polygon;
    using Drawing;

    public class UpdateMesh3DByNearestCommand : EventCommand
    {

        [Inject ( ContextKeys.CONTEXT_VIEW )]
        public GameObject contextView { get; set; }

        [Inject]
        public IEntityLookup polyLookup { set; get; }

        [Inject ( "StuffCurve" )]
        public AnimationCurve stuffCurve { set; get; }

        [Inject ( "EmptyCurve" )]
        public AnimationCurve emptyCurve { set; get; }

        [Inject ( "PhotoHelper" )]
        public PhotoHelper photoHelper { set; get; }

        public override void Execute ( )
        {
            foreach ( var viewEntityPair in polyLookup.libs )
            {

                var polyView = viewEntityPair.Key;
                var polyEntity = viewEntityPair.Value;

                var triMesh =   polyEntity.GetTriangleMesh( 0.4f);//polyEntity.BaseTriangleMesh();//

                //var vertices = RecalculateStuffVertices ( triMesh, 3F);// triMesh.Vertices.Select(v=>new Vector3((float)v.x, (float)v.y, 0)).ToArray();

                var vertices = RecalculateVerticesByCurve(triMesh, stuffCurve);

                var triangles = triMesh.Triangles.SelectMany(t=> t.vertices.Reverse().Select(v=>v.id)).ToArray();//.Reverse()

                var normals = triMesh.Vertices.Select(v=>Vector3.back);

                var bounds = triMesh.bounds;
                var l = bounds.Left;
                var b = bounds.Bottom;
                var w = bounds.Width;
                var h = bounds.Height;

                var uvs = triMesh.Vertices.Select(v => photoHelper.ConvertPositionToUV(new Vector3((float)v.x, (float)v.y, 0))).ToArray();
                //var uvs = triMesh.Vertices.Select(v=> new Vector2( (float)((v.x)),(float)( (v.y))) * 0.2F).ToArray();

                var uniMesh = new Mesh();
                uniMesh.vertices = vertices;
                uniMesh.triangles = triangles;
                uniMesh.uv = uvs;
                //uniMesh.normals = normals.ToArray ( );
                uniMesh.RecalculateNormals ( );

                //Mesh Object
                if ( !polyView.meshFilter )
                {
                    var obj = new GameObject("Mesh Filter");
                    obj.transform.parent = polyView.transform;
                    polyView.meshFilter = obj.AddComponent<MeshFilter> ( );
                    var renderer = obj.AddComponent<MeshRenderer>();
                    renderer.material = GameObject.Instantiate<Material> ( Resources.Load<Material> ( "Materials/Cloth Material" ) );

                    polyView.meshFilter.GetComponent<MeshRenderer> ( ).material.SetTexture ( "_MainTex", photoHelper.Photo );

                }
                polyView.meshFilter.sharedMesh = uniMesh;

                //var medialAxis = polyEntity.GetMedialAxis();
                polyView.glUtils.Reset ( );
                //polyView.glUtils.Paths.Add ( 
                //        medialAxis.
                //        Select ( v => new Vector3 ( ( float ) v.x, ( float ) v.y, 10 ) ).
                //        Select ( p => Camera.main.WorldToScreenPoint ( p ) ).
                //        Select ( p => new Vector3 ( p.x, p.y, 0.5f ) ).
                //        ToList ( )
                //    );

                var baseTriMesh = polyEntity.BaseTriangleMesh();

                polyView.glUtils.Paths = baseTriMesh.VonoroiFaces().Select (
                    verts => verts.Select ( v => new Vector3 ( ( float ) v.x, ( float ) v.y, 10 ) ).
                        Select ( p => Camera.main.WorldToScreenPoint ( p ) ).
                        Select ( p => new Vector3 ( p.x, p.y, 0.5f ) ).
                        ToList ( )
                ).ToList ( );
                polyView.glUtils.Enable = true;

                baseTriMesh.MedialAxis ( );

            }
        }

        private Vector3 [ ] RecalculateVerticesByCurve ( TriangleNet.Mesh mesh, AnimationCurve curve, float factor = Mathf.PI )// 3.1415926f )
        {
            int size = mesh.vertices.Count;

            //求各点距边最近距离；
            float[] nearests = new float[size];
            float furthest = float.MinValue;
            for ( int i = 0; i < size; i++ )
            {
                var point = mesh.vertices[i];
                nearests [ i ] = 0;
                if ( point.label == 0 )
                {
                    nearests [ i ] = float.MaxValue;
                    foreach ( var edge in mesh.Edges.Where ( e => e.Label != 0 ) )
                    {
                        var v0 = mesh.vertices[edge.P0];
                        var v1 = mesh.vertices[edge.P1];
                        Vector2 position;// = Vector3.zero;
                        float distance = CGAlgorithm.PointToSegementDistance(point.Vector2(), v0.Vector2(), v1.Vector2(), out position );
                        //Debug.Log ( "Distance : " + distance );
                        if ( distance < nearests [ i ] )
                            nearests [ i ] = distance;

                        if ( distance > furthest )
                            furthest = distance;
                    }
                }
            }

            // 用 y =  sqrt{1- (1-x )^2} 函数计算各点的Z值；
            Vector3[] result = new Vector3[size];
            var average = nearests.Average();


            for ( int j = 0; j < size; j++ )
            {
                var point = mesh.vertices[j];
                Vector3 pos = point.Vector2();
                var t = 1f - nearests[j] / furthest;
                pos.z = -Mathf.Sqrt ( 1f - t * t ) * average * factor;
                //var t = nearests[j] / furthest;
                //pos.z = - curve.Evaluate(t) * average * factor;
                result [ j ] = pos;
            }

            return result;

        }

        private Vector3 [ ] RecalculateUnstuffVertices ( TriangleNet.Mesh mesh )
        {
            return mesh.Vertices.Select ( v => v.Label == 0 ? new Vector3 ( ( float ) v.x, ( float ) v.y, -0.1F ) : ( Vector3 ) v.Vector2 ( ) ).ToArray ( );
        }

        float QuarterArc ( float t )
        {
            return Mathf.Sqrt ( 1f - ( 1f - t ) * ( 1f - t ) );
        }

    }
}
