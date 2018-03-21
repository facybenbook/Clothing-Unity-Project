using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using TriangleNet.Meshing;
using TriangleNet.Geometry;
using TriangleNet.Tools;
using TriangleNet.IO;
using TriangleNet.Smoothing;

using System.Linq;

public class ClothDemo : MonoBehaviour {

    public SphereCollider sphereFirst;
    public SphereCollider sphereSecond;

    float segmentLength = 0.1F;

    Contour ClacSegments ( float x0, float y0, float x1, float y1, int mark )
    {
        var xDis = x1- x0;
        var yDis = y1 - y0;

        var dis = Mathf.Sqrt(xDis * xDis + yDis * yDis);
        float n = Mathf.Round(dis / segmentLength + 0.5F);
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
        return new Contour ( result, mark );
    }

    // Use this for initialization
    void Start () {


        //var geometry = FileReader.ReadPolyFile("Assets/Plugins/Data/superior.poly");

        var p = new Polygon();

        //p.Add ( ClacSegments ( -2, -2, 2, -2, 1 ) );
        //p.Add ( ClacSegments ( 2, -2, 2, 2, 2 ) );
        //p.Add ( ClacSegments ( 2, 2, -2, 2, 3 ) );
        //p.Add ( ClacSegments ( -2, 2, -2, -2, 4 ) );

        p.Add ( ClacSegments ( -3, -3, 3, -3, 1 ) );
        p.Add ( ClacSegments ( 3, -3, 3, 3, 2 ) );
        p.Add ( ClacSegments ( 3, 3, -3, 3, 3 ) );
        p.Add ( ClacSegments ( -3, 3, -3, -3, 4 ) );

        // Add the outer box contour with boundary marker 1.
       // p.Add ( new Contour ( new Vertex [ 4 ]
       //{
       //   new Vertex(-3,-3, 1),
       //   new Vertex(3, -3, 1),
       //   new Vertex(3, 3, 1),
       //   new Vertex(-3, 3, 1)
       //}, 1 ) );

        // p.Add ( new Contour ( new Vertex [ 4 ]
        //{
        // new Vertex(-2.9,-2.9, 1),
        // new Vertex(2.9, -2.9, 1),
        // new Vertex(2.9, 2.9, 1),
        // new Vertex(-2.9, 2.9, 1)
        //}, 1 ) );

        // Add the inner box contour with boundary marker 2.
        p.Add ( new Contour ( new Vertex [ 4 ]
        {
        new Vertex(1.0, 1.0, 2),
        new Vertex(2.0, 1.0, 2),
        new Vertex(2.0, 2.0, 2),
        new Vertex(1.0, 2.0, 2)
        }, 5 )
        , new Point ( 1.5, 1.5 ) ); // Make it a hole.

        //var polygon = FileProcessor.Read("Assets/Plugins/Data/box_with_a_hole.poly");

        var options = new ConstraintOptions() { ConformingDelaunay = true };
        var quality = new QualityOptions() {MinimumAngle = 25F  , MaximumArea = 0.04F};

        var triMesh =  (TriangleNet.Mesh)p.Triangulate(options, quality);

        var smoothing = new SimpleSmoother();
        smoothing.Smooth ( triMesh );

        triMesh.Refine ( quality, true );

        triMesh.Renumber ( );

        //var bounds = new Rectangle(-1.0, -1.0, 2.0, 2.0);

        //var triMesh = GenericMesher.StructuredMesh(bounds, 20, 20);

        var vertices = triMesh.Vertices.Select(v=> new Vector3((float)v.x, (float)v.y, 0)).ToArray();
        var triangles = triMesh.Triangles.SelectMany(t=> t.vertices.Select(v=>v.id)).ToArray();//.Reverse()
        var normals = triMesh.Vertices.Select(v=>transform.forward).ToArray();

        var bounds = triMesh.bounds;
        var l = bounds.Left;
        var b = bounds.Bottom;
        var w = bounds.Width;
        var h = bounds.Height;
        var uvs = triMesh.Vertices.Select(v=> new Vector2( -(float)( (v.x - l) / w),(float)( (v.y - b) / h)) ).ToArray();
       
        var skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        var uniMesh = new Mesh ( );
        
        uniMesh.vertices = vertices;
        uniMesh.triangles = triangles;
        uniMesh.uv = uvs;
        uniMesh.normals = normals;

        skinnedRenderer.sharedMesh = uniMesh;

        var cloth = GetComponent<Cloth>();
        if ( !cloth )
        {
            cloth = gameObject.AddComponent<Cloth> ( );
        }

        var coes = new List<ClothSkinningCoefficient>();

        foreach ( var v in vertices )
        {
            coes.Add ( new ClothSkinningCoefficient ( ) { maxDistance = 10 , collisionSphereDistance = 0.1F} );
        }

        cloth.coefficients = coes.ToArray ( );

        ClothSphereColliderPair pair = new ClothSphereColliderPair(sphereFirst, null);
        cloth.sphereColliders = new ClothSphereColliderPair [ ] { pair };

        //cloth.externalAcceleration = new Vector3 ( 0, -10, 0 );

        cloth.stretchingStiffness = 0.9F;
        cloth.bendingStiffness = 0.1F;

        cloth.collisionMassScale = 0.1F;
        cloth.friction = 1F;

        cloth.SetEnabledFading ( true );
        cloth.sleepThreshold = 0.1F;

        cloth.damping = 0.2F;

    }
	
}
