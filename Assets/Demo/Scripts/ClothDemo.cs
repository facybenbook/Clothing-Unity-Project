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

    

    // Use this for initialization
    void Start () {


        //var geometry = FileReader.ReadPolyFile("Assets/Plugins/Data/superior.poly");

        var p = new Polygon();

        // Add the outer box contour with boundary marker 1.
        p.Add ( new Contour ( new Vertex [ 4 ]
        {
        new Vertex(0.0, 0.0, 1),
        new Vertex(3.0, 0.0, 1),
        new Vertex(3.0, 3.0, 1),
        new Vertex(0.0, 3.0, 1)
        }, 1 ) );

        // Add the inner box contour with boundary marker 2.
        p.Add ( new Contour ( new Vertex [ 4 ]
        {
        new Vertex(1.0, 1.0, 2),
        new Vertex(2.0, 1.0, 2),
        new Vertex(2.0, 2.0, 2),
        new Vertex(1.0, 2.0, 2)
        }, 2 )
        , new Point ( 1.5, 1.5 ) ); // Make it a hole.

        //var polygon = FileProcessor.Read("Assets/Plugins/Data/box_with_a_hole.poly");

        var options = new ConstraintOptions() { ConformingDelaunay = true };
        var quality = new QualityOptions() {MinimumAngle = 25F  , MaximumArea = 0.1F};

        var triMesh =  (TriangleNet.Mesh)p.Triangulate(options, quality);

        var smoothing = new SimpleSmoother();
        smoothing.Smooth ( triMesh );

        triMesh.Refine ( quality );

        //var statistic = new QualityMeasure();

        //statistic.Update ( triMesh );

        //double area = 0.5 * statistic.AreaMaximum;

        triMesh.Renumber ( );

        //var bounds = new Rectangle(-1.0, -1.0, 2.0, 2.0);

        //var triMesh = GenericMesher.StructuredMesh(bounds, 20, 20);

        var vertices = triMesh.Vertices.Select(v=>new Vector3((float)v.x, (float)v.y, 0)).ToArray();

        var triangles = triMesh.Triangles.SelectMany(t=> t.vertices.Select(v=>v.id).Reverse()).ToArray();

        var bounds = triMesh.bounds;
        var l = bounds.Left;
        var b = bounds.Bottom;
        var w = bounds.Width;
        var h = bounds.Height;
        var uvs = triMesh.Vertices.Select(v=> new Vector2( (float)((v.x - l)/w),(float)( (v.y - b) / h)) ).ToArray();
       

        var skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        var uniMesh = skinnedRenderer.sharedMesh;
        //var uniMesh = GetComponent<MeshFilter>().mesh;
        if ( !uniMesh )
        {
            uniMesh = new Mesh ( );
            skinnedRenderer.sharedMesh = uniMesh;
        }
        uniMesh.vertices = vertices;
        uniMesh.triangles = triangles;
        uniMesh.uv = uvs;
        uniMesh.RecalculateNormals ( );

        var cloth = GetComponent<Cloth>();
        if ( !cloth )
        {
            cloth = gameObject.AddComponent<Cloth> ( );
        }

        var coes = new List<ClothSkinningCoefficient>();

        foreach ( var v in vertices )
        {
            if ( v.y > 2.9F )
            {
                coes.Add ( new ClothSkinningCoefficient ( ) { maxDistance = 0, collisionSphereDistance = 0 } );
            }
            else
            {
                coes.Add ( new ClothSkinningCoefficient ( ) { maxDistance = 1, collisionSphereDistance = 1 } );
            }
        }

        cloth.coefficients = coes.ToArray ( );

    }
	
}
