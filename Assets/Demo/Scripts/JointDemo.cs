using System.Collections;
using System.Collections.Generic;
using UnityEngine;



using TriangleNet.Meshing;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.IO;
using TriangleNet.Smoothing;
using TriangleNet.Voronoi;
using TriangleNet.Topology;//.DCEL;
using System.Linq;

using TriangleNet.Extensions;

using UnityEngine.EventSystems;

public class JointDemo : MonoBehaviour//, IDragHandler, IPointerDownHandler, IPointerUpHandler
{

    const float KGSM = 0.2F; // 200g

    public float Size = 3F;

    //public MeshFilter backMesh;

    GameObject Root;

    //Rigidbody Body = null;
    //Vector3 lastPosition = Vector3.zero;

    //public void OnPointerDown ( PointerEventData eventData )
    //{
    //    var position = eventData.pointerCurrentRaycast.worldPosition;
    //    if ( !Body )
    //    {
    //        lastPosition = position;
    //        float minDis = float.MaxValue;

    //        foreach ( var body in Root.GetComponentsInChildren<Rigidbody> ( ) )
    //        {
    //            var bodyPos = body.transform.position;
    //            var dis = Vector3.Distance(position, bodyPos);
    //            if ( dis < minDis ) Body = body;
    //        }
    //    }
    //}

    //public void OnDrag ( PointerEventData eventData )
    //{

    //    if ( Body )
    //    {
    //        var position = eventData.pointerCurrentRaycast.worldPosition;
    //        var force =( position - lastPosition).normalized * 0.5F;

    //        Debug.Log ( Body.name + ", " + force );
    //        Body.AddForce ( force );

    //        lastPosition = position;
    //    }
    //}

    //public void OnPointerUp ( PointerEventData eventData )
    //{
    //    Body = null;
    //}

    //float segmentLength = 0.2F;
    public int partsNum = 13;

    public List<Rigidbody> rigid_Up = new List<Rigidbody>();
    public List<Rigidbody> rigid_Down = new List<Rigidbody>();
    public List<Rigidbody> rigid_Left = new List<Rigidbody>();
    public List<Rigidbody> rigid_Right = new List<Rigidbody>();

    public List<Rigidbody> others = new List<Rigidbody>();

    public List<Rigidbody> All = new List<Rigidbody>();

    Contour CosSegements (float x0, float y0, float x1, float y1, int n, int mark )
    {
        var result = new List<Vertex>();
        
        var delta =Mathf.PI/n;

        Debug.Log ("Delta cos value : " +  Mathf.Cos ( delta ) );
        
        for(float  i = 0, angle = 0; i<n; i++, angle+=delta  )
        {
            var factor = (1.0F + Mathf.Cos(angle)) / 2F;
            var x = Mathf.Lerp(x0, x1, factor);
            var y = Mathf.Lerp(y0, y1, factor); //center_y + radius_y * Mathf.Cos (angle);
            result.Add ( new Vertex ( x, y, mark ) );
        }
        return new Contour ( result );
    }

    Contour ClacSegments ( float x0, float y0, float x1, float y1, int n, int mark )
    {
        var xDis = x1- x0;
        var yDis = y1 - y0;

        var dis = Mathf.Sqrt(xDis * xDis + yDis * yDis);
        //float n = Mathf.Round(dis / segmentLength);
        float dX = xDis / n;
        float dY = yDis / n;
        //var dir = (end - start).normalized;
        var result = new List<Vertex>();
        //var current = start;
        var x = x0;
        var y = y0;
        for ( int i = 0; i <= n - 1; i++ )
        {
            result.Add ( new Vertex ( x, y, mark ) );

            x += dX;
            y += dY;
        }
        return new Contour ( result );
    }

    // Use this for initialization
    void Start ( )
    {
        var p = new Polygon();

        p.Add ( CosSegements ( -Size, -Size, Size, -Size, partsNum, 1 ) );
        p.Add ( CosSegements ( Size, -Size, Size, Size, partsNum, 2 ) );
        p.Add ( CosSegements ( Size, Size, -Size, Size, partsNum, 3 ) );
        p.Add ( CosSegements ( -Size, Size, -Size, -Size, partsNum, 4 ) );
        
        //var polygon = FileProcessor.Read("Assets/Plugins/Data/box_with_a_hole.poly");

        var options = new ConstraintOptions() { ConformingDelaunay = true };
        var quality = new QualityOptions() {MinimumAngle = 25F , MaximumArea = 0.1F};

        var triMesh =  (TriangleNet.Mesh)p.Triangulate(options, quality);

        var smoothing = new SimpleSmoother();
        smoothing.Smooth ( triMesh );

        triMesh.Refine ( quality );

        triMesh.Renumber ( );

        int boneIndex0 = 0;
        var bindposes = new List<Matrix4x4>();
        var bones = new List<Transform>();

        var fBoneWeights = new List<BoneWeight>();

        var vertexLookup = new Dictionary<Point, Rigidbody>();

        Root = new GameObject ( );
        Root.transform.parent = transform;
        Root.transform.localRotation = Quaternion.identity;

        var voronoi = new StandardVoronoi(triMesh);
        
        var triAreas = triMesh.Triangles.Sum(t => t.Area());
        Debug.Log ( "Tri Areas : " + triAreas );

        Debug.Log ( "Vor Count : " + voronoi.Faces.Count );
        float SumArea = 0F;
        foreach ( var face in voronoi.Faces )
        {
            var origins = face.GetAllVertices();
            //var origins = face.EnumerateEdges().Select(e => (Point) e.Origin);

            if ( origins.Count() > 0 )
            {
                SumArea += origins.Area ( );
                var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);//new GameObject();
                obj.transform.localScale = Vector3.one * 0.005F;
                var renderer = obj.GetComponent<MeshRenderer> ( );
                if ( renderer )
                {
                    renderer.enabled = false;
                }

                //obj.GetComponent<SphereCollider> ( ).enabled = false;

                var center = face.generator;// origins.MassCenter();
                var worldPosition = transform.TransformPoint(new Vector3 ( ( float ) center.x, ( float ) center.y, 0 ));
                obj.transform.position = worldPosition;
                obj.transform.parent = Root.transform;
                bones.Add ( obj.transform );
                bindposes.Add ( obj.transform.worldToLocalMatrix * transform.localToWorldMatrix );

                var weight = new BoneWeight();

                weight.boneIndex0 = boneIndex0;
                weight.weight0 = 1.0F;
                fBoneWeights.Add ( weight );


                var rigidbody = obj.AddComponent<Rigidbody>();
                rigidbody.useGravity = false;
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                rigidbody.mass = origins.Area ( ) * KGSM;
                rigidbody.drag = 4;
                rigidbody.angularDrag = 2;


                rigidbody.constraints ^= RigidbodyConstraints.FreezeRotation;

                //if ( center.y > 1.9 )                    rigidbody.isKinematic = true;
                if ( center.y == Size ) rigid_Up.Add ( rigidbody );
                else if ( center.y == -Size ) rigid_Down.Add ( rigidbody );
                else if ( center.x == Size ) rigid_Right.Add ( rigidbody );
                else if ( center.x == -Size ) rigid_Left.Add ( rigidbody );
                else others.Add ( rigidbody );

                All.Add ( rigidbody );

                    vertexLookup.Add ( center, rigidbody );

                boneIndex0++;
            }
            
            //SkinnedMeshRenderer.BakeMesh

        }
        
        foreach ( var edge in triMesh.Edges )
        {
            var v0 =   triMesh.Vertices.ElementAt(edge.P0);
            var v1 =  triMesh.Vertices.ElementAt(edge.P1);
            var pt_0 = new Vector2((float) v0.x, (float)v0.y);
            var pt_1 = new Vector2((float) v1.x, (float)v1.y);
            var distance = Vector2.Distance(pt_0, pt_1);

            var rigid_0 =vertexLookup[v0];
            var rigid_1 = vertexLookup[v1];

            var spring = rigid_0.gameObject.AddComponent<SpringJoint>();
            spring.connectedBody = rigid_1;
            spring.minDistance = distance * .96F;
            spring.maxDistance = distance * 1.00F;
            spring.spring = 8F;
            spring.damper = 0F;
            spring.autoConfigureConnectedAnchor = false;
            spring.enableCollision = false;
            spring.connectedAnchor = spring.anchor = Vector3.zero;
            spring.axis = Vector3.back;
            spring.tolerance = 0.01F;
            spring.enablePreprocessing = false;
        }
        
        var vertices = triMesh.Vertices.Select(v=>new Vector3((float)v.x, (float)v.y, 0)).ToArray();

        var triangles = triMesh.Triangles.SelectMany(t=> t.vertices.Select(v=>v.id)).ToArray();//.Reverse()

        var normals = triMesh.Vertices.Select(v=>transform.forward);

        var bounds = triMesh.bounds;
        var l = bounds.Left;
        var b = bounds.Bottom;
        var w = bounds.Width;
        var h = bounds.Height;
        var uvs = triMesh.Vertices.Select(v=> new Vector2( -(float)((v.x - l)/w),(float)( (v.y - b) / h)) ).ToArray();

        Debug.Log ( string.Format ( "Vertices : {0}, Edge : {1}, Segments : {2}, Triangles : {3}, Holes : {4}",
            triMesh.Vertices.Count, triMesh.Edges.Count ( ), triMesh.Segments.Count, triMesh.Triangles.Count, triMesh.Holes.Count ) );

        var skinnedRenderer = GetComponent<SkinnedMeshRenderer>();

        //var meshFilter = GetComponent<MeshFilter>();
        //if ( !meshFilter )
        //{
        //    meshFilter = gameObject.AddComponent<MeshFilter> ( );
        //}
        var uniMesh = new Mesh();
        uniMesh.vertices = vertices;
        uniMesh.triangles = triangles;
        uniMesh.uv = uvs;
        uniMesh.normals = normals.ToArray ( );

        uniMesh.boneWeights = fBoneWeights.ToArray ( );
        uniMesh.bindposes = bindposes.ToArray ( );

        skinnedRenderer.sharedMesh = uniMesh;
        skinnedRenderer.bones = bones.ToArray ( );
        skinnedRenderer.rootBone = Root.transform;

        //GetComponent<MeshCollider> ( ).sharedMesh = uniMesh;
    }

    //void OnGUI ( )
    //{
    //    if ( GUI.Button (new Rect(0, 0, 80, 32), "Bake" ) )
    //    {
    //        var mesh = new Mesh();
    //        GetComponent<SkinnedMeshRenderer> ( ).BakeMesh ( mesh );
    //        backMesh.mesh = mesh;

    //    }
    //}

}
