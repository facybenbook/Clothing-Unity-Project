using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using TriangleNet.Tools;
using TriangleNet.IO;

using System.Linq;

public class UnitTest : MonoBehaviour {

    

    // Use this for initialization
    void Start () {


        var geometry = FileReader.ReadPolyFile("Assets/Plugins/Data/superior.poly");
        //var circle = new TriangleNet.Data.
        //var geometry =  circle.

        var triMesh = new TriangleNet.Mesh ( );
        /*_mesh.Triangulate("Assets/Plugins/Data/superior.poly");*/

        triMesh.behavior.Quality = true;
        triMesh.behavior.MinAngle = 33.8f;
        triMesh.Triangulate ( geometry );
        //TriMesh.Refine(true);
        // Get mesh statistics.
        var statistic = new Statistic();
        statistic.Update ( triMesh, 1 );

        // Refine by setting a custom maximum area constraint.
        triMesh.Refine ( statistic.LargestArea / 4 );
        triMesh.Smooth ( );

        Debug.Log ( string.Format ( "width {0:0.00}, height {1:0.00}; min {2:0.00}, {3:0.00}; max {4:0.00}, {5:0.00}",
            triMesh.Bounds.Width, triMesh.Bounds.Height,
            triMesh.Bounds.Xmin, triMesh.Bounds.Ymin,
            triMesh.Bounds.Xmax, triMesh.Bounds.Ymax ) );

        triMesh.Renumber ( );

        int n = triMesh.Vertices.Count;

        var vertices = triMesh.Vertices.Select(v=>new Vector3((float)v.x, (float)v.y, 0)).ToArray();

        var triangles = triMesh.Triangles.SelectMany(t=> new int[]{t.P0, t.P1, t.P2 }).ToArray();

        var uniMesh = GetComponent<MeshFilter>().mesh;
        if ( !uniMesh )
        {
            uniMesh = new Mesh ( );
            GetComponent<MeshFilter> ( ).mesh = uniMesh;
        }
        uniMesh.vertices = vertices;
        uniMesh.triangles = triangles;

    }
	
}
