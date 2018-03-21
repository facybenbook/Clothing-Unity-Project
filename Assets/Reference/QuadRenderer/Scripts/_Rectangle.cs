using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class _Rectangle : _Quadrangle
{

    //   
    //  up(y)			1---------width---------2
    //  ^				|						|
    //  |				|					  length
    //  |				|						|
    //  .--->right(x)	0-----------------------3
    //

    public float width = 1.0f;
    public float blankCap = 1.0f;
    public Vector2 startPosition;
    public Vector2 endPosition;
    

    public void SetEndsPosition(Vector2 startPos, Vector2 endPos)
    {
        startPosition = startPos;
        endPosition = endPos;
    }

    protected override void HandleOnUpdateVertices()
    {
        float length = Vector3.Distance(startPosition, endPosition) - blankCap * 2;
        float halfLength = length * 0.5f;
        float halfWidth = width * 0.5f;

        Vector3 position = (endPosition + startPosition) * 0.5f;
        float angle = Mathf.Atan2(endPosition.x - startPosition.x, endPosition.y - startPosition.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.back);

        transform.localPosition = position;
        transform.localRotation = rotation;
        
        List<Vector3> vertices = new List<Vector3>(){
            (halfWidth * Vector3.left + Vector3.down * halfLength),
            (halfWidth * Vector3.left + Vector3.up * halfLength),
            (halfWidth * Vector3.right + Vector3.up * halfLength),
            (halfWidth * Vector3.right + Vector3.down * halfLength)
        };
           
        Vertices = vertices.ToArray();

        float f = length / width;

        Vector2[] uv = new Vector2[] {
            new Vector2(0, 0),
        new Vector2(0, f),
        new Vector2(1, f),
        new Vector2(1, 0)
        };

        UV = uv;
    }

    protected override void HandleOnInitial()
    {
        Name = "Rectangle";
        //IsUpdateAtRuntime = true;
    }

}
