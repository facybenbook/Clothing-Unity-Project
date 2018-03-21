using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using UnityEngine.EventSystems;

using Clothing.Polygon;

public class SketchDemo : MonoBehaviour {

    PointView pointPrefab;
    LineView linePrefab;
    

    public float size = 2.0F;



    void Start ( )
    {
        var pointAsset = Resources.Load("Prefabs/Point Entity") as GameObject;
        pointPrefab = pointAsset.GetComponent<PointView> ( );
        pointPrefab.CreatePool ( );

        var lineAsset = Resources.Load("Prefabs/Line Entity") as GameObject;
        linePrefab = lineAsset.GetComponent<LineView> ( );
        linePrefab.CreatePool ( );

        init ( );
    }

    public void init ( )
    {
        var positions = new Vector3[] { new Vector3(size, size), new Vector3(size, -size), new Vector3(-size, -size), new Vector3(-size, size)};

        List<PointView> points = new List<PointView>();
        foreach(var pos in positions )
        {
            var point = pointPrefab.Spawn ( pos );
            point.transform.parent = transform;

            points.Add ( point );
        }

        for(int i = 0, j = positions.Length - 1; i<positions.Length; j = i++ )
        {
            var line = linePrefab.Spawn();
            line.transform.parent = transform;

            line.startPoint = points [ i ];
            line.endPoint = points [ j ];
            line.OnRender ( );

            
        }

    }
    
}
