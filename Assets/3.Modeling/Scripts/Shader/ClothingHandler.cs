using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.MassSpringSystem
{

    using System.Linq;

    using Polygon;

    using TriangleNet.Geometry;
    using TriangleNet.Meshing;
    using TriangleNet.Smoothing;
    using TriangleNet.Voronoi;
    using TriangleNet.Extensions;

    using strange.extensions.command.impl;
    using strange.extensions.context.api;



    public static class ClothingShaderProperties
    {
        public const string DeltaTimeName = "deltaTime";

        public const string CenterName = "center";

        public const string DampingName = "damping";
        public const string SpringStiffnessName = "stiffness";

        public const string MassBufferName = "massBuffer";
        public const string SpringBufferName = "springBuffer";

        public const string InitForcesKernel = "CSInitForces";
        public const string UpdateForcesKernel = "CSUpdateForces";
        public const string UpdatePositionKernel = "CSUpdatePosition";
    }



    public class ClothingHandler : MonoBehaviour
    {
        public struct Mass
        {

            public float mass;

            public int id;
            public int label;

            public Vector3 nor;
            public Vector3 pos;
            public Vector3 vel;
            public Vector3 force;

        }

        public struct Spring
        {
            public int m0;
            public int m1;
            public float length;
            //public float stiffness;
        }

        private ComputeShader clothingShader;

        [Range(0.01f, 0.999f)]
        public float Damping         = 0.05f;

        [Range(0.1f, 100.0f)]
        public float SpringStiffness = 2.0f;

        private Vector3 Center;

        private ComputeBuffer massBuffer;
        private ComputeBuffer springBuffer;

        private int initForcesKernel;
        private int updateForcesKernel;
        private int updatePosKernel;


        const float KGSM = 1F;

        int massesCount = 0;
        int springsCount = 0;

        bool hasCreated = false;

        public void OnCreate ( TriangleNet.Mesh triMesh )
        {
            CreateShader ( );

            ResetBuffers ( triMesh );

            CreateMesh ( triMesh );

            hasCreated = true;

            StartCoroutine ( WaitForStuffing ( ) );
        }

        private void CreateShader ( )
        {
            clothingShader = Resources.Load<ComputeShader> ( "ClothingShader" );
            initForcesKernel = clothingShader.FindKernel ( ClothingShaderProperties.InitForcesKernel );
            updateForcesKernel = clothingShader.FindKernel ( ClothingShaderProperties.UpdateForcesKernel );
            updatePosKernel = clothingShader.FindKernel ( ClothingShaderProperties.UpdatePositionKernel );
        }

        private void ResetBuffers ( TriangleNet.Mesh triMesh )
        {
            var bounds = triMesh.Bounds;
            var x = (float) (bounds.Left + bounds.Right) * 0.5F;
            var y = (float) (bounds.Top + bounds.Bottom) * 0.5F;
            Center = new Vector3 ( x, y, 0 );
            //Mass : Vertex
            massesCount = triMesh.Vertices.Count;

            List<Mass> masses = new List<Mass>();//new Mass [massesCount];

            var voronoi = new StandardVoronoi(triMesh);

            foreach ( var vertex in triMesh.Vertices )
            //for(int i =0; i < vertCount; i++ )
            {

                var mass = new Mass();
                mass.id = vertex.id;//0, 1, 2, ... index
                mass.label = vertex.label;//1, 1, ..., 2, 2, ..., 3, 3, ..., 4, 4, ..., 0, 0, 0 ...
                mass.nor = Vector3.back;//new Vector3 ( ( float ) vertex.x, ( float ) vertex.y, 0 );
                //var z = vertex.label == 0 ? -0.1F : 0.0F;
                mass.pos = new Vector3 ( ( float ) vertex.X, ( float ) vertex.Y, 0 );
                mass.vel = Vector3.zero;
                mass.force = Vector3.zero;

                var face = voronoi.Faces [ vertex.id ];
                var origins = face.GetAllVertices();
                mass.mass =  ( origins.Count > 0 ? origins.Area ( ) : 0 )  * KGSM;

                //masses [ vertex.id ] = mass;
                masses.Add ( mass );
            }

            //如果 顶点数量 不能被十整除 则 增加 不足 部分；
            if ( massesCount % 10 > 0 )
            {
                massesCount += 10 - ( massesCount % 10 );
                for ( int k = triMesh.vertices.Count; k < massesCount; k++ )
                {
                    masses.Add ( new Mass ( ) { id = -1 } );
                }
            }

            massBuffer = new ComputeBuffer ( massesCount, sizeof ( float ) * 13 + sizeof ( int ) * 2 );

            massBuffer.SetData ( masses.ToArray ( ) );

            //Spring : Edge + Shear spring


            //Spring[] springs = new Spring[edgeCount];
            List<Spring> springs = new List<Spring>();

            //int  i = 0;
            foreach ( var edge in triMesh.Edges )
            {
                var spring = new Spring();
                spring.m0 = edge.P0;
                spring.m1 = edge.P1;

                var v0 =   triMesh.Vertices.ElementAt(edge.P0);
                var v1 =  triMesh.Vertices.ElementAt(edge.P1);
                //bool isBounds = v0.Label != 0 && v1.Label != 0;
                var pt_0 = new Vector2((float) v0.x, (float)v0.y);
                var pt_1 = new Vector2((float) v1.x, (float)v1.y);
                spring.length = Vector2.Distance ( pt_0, pt_1 );
                //bool isEdge  = v0.label != 0 && v1.label != 0;
                //spring.stiffness =10.0F;

                //springs [ i ] = spring;
                springs.Add ( spring );

                //i++;
            }

            foreach ( var triangle in triMesh.Triangles )
            {
                for ( int j = 0; j < 3; j++ )
                {
                    var currID = triangle.GetVertexID(j);
                    var nextID = triangle.GetVertexID( ( j + 1 ) % 3 );
                    var preID = triangle.GetVertexID( ( j + 2 ) % 3 );
                    var neighbor = triangle.GetNeighbor( j );

                    if ( neighbor != null )
                    {
                        var ids = new int[3] {neighbor.GetVertexID ( 0 ), neighbor.GetVertexID ( 1 ), neighbor.GetVertexID ( 2 ) };
                        var oppID = ids.FirstOrDefault( id => id != nextID && id != preID);

                        bool isContants = springs.Any(s => (s.m0 == currID && s.m1 == oppID) || (s.m1 == currID && s.m0 == oppID));

                        if ( !isContants )
                        {
                            var spring = new Spring();
                            spring.m0 = currID;
                            spring.m1 = oppID;

                            var v0 =   triMesh.Vertices.ElementAt(currID);
                            var v1 =  triMesh.Vertices.ElementAt(oppID);
                            //bool isBounds = v0.Label != 0 && v1.Label != 0;
                            var pt_0 = new Vector2((float) v0.x, (float)v0.y);
                            var pt_1 = new Vector2((float) v1.x, (float)v1.y);
                            spring.length = Vector2.Distance ( pt_0, pt_1 );
                            //bool isEdge  = v0.label != 0 && v1.label != 0;
                            //spring.stiffness = 10.0F;
                            springs.Add ( spring );
                        }
                    }
                }
            }

            springsCount = springs.Count;
            if ( springsCount % 10 > 0 )
            {

                springsCount += 10 - ( springsCount % 10 );
                for ( int l = springs.Count; l < springsCount; l++ )
                {
                    springs.Add ( new Spring ( ) { m0 = 0, m1 = 0, length = 0 } );//, stiffness = 0 } );
                }
            }


            springBuffer = new ComputeBuffer ( springsCount, sizeof ( float ) * 1 + sizeof ( int ) * 2 );
            springBuffer.SetData ( springs.ToArray ( ) );

            clothingShader.SetBuffer ( initForcesKernel, ClothingShaderProperties.MassBufferName, massBuffer );
            clothingShader.SetBuffer ( updateForcesKernel, ClothingShaderProperties.MassBufferName, massBuffer );
            clothingShader.SetBuffer ( updateForcesKernel, ClothingShaderProperties.SpringBufferName, springBuffer );
            clothingShader.SetBuffer ( updatePosKernel, ClothingShaderProperties.MassBufferName, massBuffer );


            clothingShader.SetFloat ( ClothingShaderProperties.DampingName, Damping );
            clothingShader.SetFloat ( ClothingShaderProperties.SpringStiffnessName, SpringStiffness );
            clothingShader.SetVector ( ClothingShaderProperties.CenterName, Center );
        }


        private void CreateMesh ( TriangleNet.Mesh triMesh )
        {
            var vertices = triMesh.Vertices.Select(v=>new Vector3((float)v.x, (float)v.y, 0)).ToArray();

            var triangles = triMesh.Triangles.SelectMany(t=> t.vertices.Reverse().Select(v=>v.id)).ToArray();//.Reverse()

            var normals = triMesh.Vertices.Select(v=>Vector3.back);

            var bounds = triMesh.bounds;
            var l = bounds.Left;
            var b = bounds.Bottom;
            var w = bounds.Width;
            var h = bounds.Height;

            var uvs = triMesh.Vertices.Select(v=> new Vector2( (float)((v.x)),(float)( (v.y))) * 0.2F).ToArray();

            var uniMesh = new Mesh();
            uniMesh.vertices = vertices;
            uniMesh.triangles = triangles;
            uniMesh.uv = uvs;
            uniMesh.normals = normals.ToArray ( );

            MeshFilter meshFilter = GetComponent<MeshFilter>();
            //Mesh Object
            if ( !meshFilter )
            {
                return;
            }

            meshFilter.mesh = uniMesh;
        }

        private void ReleaseBuffers ( )
        {
            if ( massBuffer != null )
                massBuffer.Release ( );
            if ( springBuffer != null )
                springBuffer.Release ( );
        }

        private void Dispatch ( )
        {
            clothingShader.SetFloat ( ClothingShaderProperties.DeltaTimeName, Time.deltaTime );

            clothingShader.SetBuffer ( initForcesKernel, ClothingShaderProperties.MassBufferName, massBuffer );
            clothingShader.Dispatch ( initForcesKernel, massesCount / 10, 1, 1 );

            clothingShader.SetBuffer ( updateForcesKernel, ClothingShaderProperties.MassBufferName, massBuffer );
            clothingShader.Dispatch ( updateForcesKernel, springsCount / 10, 1, 1 );
            
            clothingShader.SetBuffer ( updatePosKernel, ClothingShaderProperties.MassBufferName, massBuffer );
            clothingShader.Dispatch ( updatePosKernel, massesCount / 10, 1, 1 );

        }

        private void UpdateMesh ( )
        {
            Mass[] masses = new Mass[massesCount];
            massBuffer.GetData ( masses );
            var positions = masses.Where(m => m.id >= 0).OrderBy(m => m.id).Select(m => m.pos).ToArray();
            var mesh = GetComponent<MeshFilter>().mesh;
            mesh.vertices = positions;

            mesh.RecalculateNormals ( );
            var normals = mesh.normals;
            for ( int i = 0; i < normals.Length; i++ )
            {
                masses [ i ].nor = normals [ i ];
            }
            massBuffer.SetData ( masses );
        }

        // Use this for initialization
        void Start ( )
        {

        }

        // Update is called once per frame
        void Update ( )
        {

            ///????
            ///
            //if ( !hasCreated ) return;

            //Dispatch ( );
            //UpdateMesh ( );

        }

        IEnumerator WaitForStuffing ( )
        {
            int count = 0;
            while ( count < 128 )// 94 )
            {
                Dispatch ( );
                UpdateMesh ( );
                count++;
                yield return new WaitForFixedUpdate ( );
            }


            yield return null;
        }

        void OnDestroy ( )
        {
            ReleaseBuffers ( );
            //Resources.UnloadUnusedAssets ( );
        }
    }

}