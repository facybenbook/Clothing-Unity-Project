using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class _RectExtensions
{
    public static Vector2[] Vector2Array(this Rect rect)
    {
        return new Vector2[]{
                    new Vector2(rect.xMin, rect.yMin),
                    new Vector2(rect.xMin, rect.yMax),
                    new Vector2(rect.xMax, rect.yMax),
                    new Vector2(rect.xMax,  rect.yMin)
            };
    }

    public static Vector2[] Vector2AllArray(this Rect rect)
    {
        return new Vector2[]{
            new Vector2(rect.xMin, rect.yMax),
            new Vector2(rect.xMax, rect.yMax),
            new Vector2(rect.xMin, rect.yMin), 
            new Vector2(rect.xMax,  rect.yMin),
            rect.center
            };
    }

    public static Vector3[] Vector3Array(this Rect rect)
    {
        return new Vector3[]{
                    new Vector2(rect.xMin, rect.yMin),
                    new Vector2(rect.xMin, rect.yMax),
                    new Vector2(rect.xMax, rect.yMax),
                    new Vector2(rect.xMax,  rect.yMin)
            };
    }

    public static List<Vector2> Vector2List(this Rect rect)
    {
        return new List<Vector2>(rect.Vector2Array());
    }

    public static List<Vector3> Vector3List(this Rect rect)
    {
        return new List<Vector3>(rect.Vector3Array());
    }

    public static Rect Append(this Rect src, Rect parts )
    {

        if ( src.size == Vector2.zero ) return parts;

        var min_x = Mathf.Min(src.xMin, parts.xMin);
        var max_x = Mathf.Max(src.xMax, parts.xMax);
        var min_y = Mathf.Min(src.yMin, parts.yMin);
        var max_y = Mathf.Max(src.yMax, parts.yMax);
        return UnityEngine.Rect.MinMaxRect ( min_x, min_y, max_x, max_y );
    }

    public static Rect Rect(this List<Vector2> vertices)
    {
        var min_x = vertices.Min(p => p.x);
        var max_x = vertices.Max(p => p.x);
        var min_y = vertices.Min(p => p.y);
        var max_y = vertices.Max(p => p.y);

        return UnityEngine.Rect.MinMaxRect( min_x, min_y, max_x, max_y);
    }

    public static Rect Rect(this List<Vector3> vertices)
    {
        var min_x = vertices.Min(p => p.x);
        var max_x = vertices.Max(p => p.x);
        var min_y = vertices.Min(p => p.y);
        var max_y = vertices.Max(p => p.y);

        return UnityEngine.Rect.MinMaxRect( min_x, min_y, max_x, max_y);
    }

    public static Rect Rect(this IEnumerable<Vector2> vertices )
    {
        var min_x = vertices.Min(p => p.x);
        var max_x = vertices.Max(p => p.x);
        var min_y = vertices.Min(p => p.y);
        var max_y = vertices.Max(p => p.y);

        return UnityEngine.Rect.MinMaxRect ( min_x, min_y, max_x, max_y );
    }
    
    public static Rect Rect ( this IEnumerable<IEnumerable<Vector2>> polygons )
    {
        var vertices = polygons.SelectMany(poly => poly.Select(v=>v));
        
        return vertices.Rect ( );
    }

    public static Rect Rect ( this IEnumerable<IEnumerable<Vector3>> polygons )
    {
        var vertices = polygons.SelectMany(poly => poly.Select(v=>(Vector2)v));
        
        return vertices.Rect ( );
    }

    public static Rect Rect ( this List<List<Vector2>> polygons )
    {
        var vertices = polygons.SelectMany(poly => poly.Select(v=>v));
       
        return vertices.Rect ( );
    }

    public static Rect Offset(this Rect srcRect, float factor )
    {
        var max = Mathf.Max(srcRect.width, srcRect.height);

        var spacing = max * (factor - 1.0F);

        return UnityEngine.Rect.MinMaxRect(srcRect.min.x - spacing, srcRect.min.y - spacing, srcRect.max.x + spacing, srcRect.max.y + spacing);
    }

    public static bool ContainOthers ( this Rect poly, Rect other )
    {
        bool result = true;
        foreach ( var point in other.Vector2Array ( ) )
        {
            result &= poly.Contains ( point );
        }
        return result;
    }
}
