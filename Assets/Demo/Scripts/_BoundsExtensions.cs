using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class _BoundsExtensions
{

    public static Bounds MinMaxBounds ( Vector3 min, Vector3 max )
    {
        var center = (Vector3)(max + min) * 0.5F;
        var size = (Vector3)(max - min);
        return new Bounds ( center, size );
    }

    public static bool IsNullOrEmpty ( this Bounds bounds )
    {
        return bounds == null || bounds.size == Vector3.zero;
    }



    /// <summary>
    /// 返回当前方向下 Bounds 的平面
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    //public static Plane FaceTo ( this Bounds bounds, Direction direction )
    //{
    //    switch ( direction )
    //    {
    //        case Direction.RIGHT:
    //            return new Plane ( Vector3.right, bounds.max );
    //        case Direction.UP:
    //            return new Plane ( Vector3.up, bounds.max );
    //        case Direction.FORWARD:
    //            return new Plane ( Vector3.forward, bounds.max );
    //        case Direction.LEFT:
    //            return new Plane ( Vector3.left, bounds.min );
    //        case Direction.DOWN:
    //            return new Plane ( Vector3.down, bounds.min );
    //        case Direction.BACK:
    //            return new Plane ( Vector3.back, bounds.min );
    //        default:
    //            return new Plane ( Vector3.forward, bounds.max );
    //    }
    //}

    /// <summary>
    /// 返回 该 Bounds 通过 min, max 点的所有的六个面；
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public static Plane [ ] AllPlanes ( this Bounds bounds )
    {
        return new Plane [ ] {
            new Plane(Vector3.right, bounds.max),
            new Plane(Vector3.up, bounds.max),
            new Plane(Vector3.forward, bounds.max),
            new Plane(Vector3.left, bounds.min),
            new Plane(Vector3.down, bounds.min),
            new Plane(Vector3.back, bounds.min)
        };
    }

    public static Plane [ ] AllPlanesByCenter ( this Bounds bounds )
    {
        return new Plane [ ] {
            new Plane(Vector3.right, bounds.center),
            new Plane(Vector3.up, bounds.center),
            new Plane(Vector3.forward, bounds.center),
            new Plane(Vector3.left, bounds.center),
            new Plane(Vector3.down, bounds.center),
            new Plane(Vector3.back, bounds.center)
        };
    }

    public static Bounds Bounds ( this IEnumerable<Vector3> vertices )
    {
        var min_x = vertices.Min(p => p.x);
        var max_x = vertices.Max(p => p.x);
        var min_y = vertices.Min(p => p.y);
        var max_y = vertices.Max(p => p.y);
        var min_z = vertices.Min(p => p.z);
        var max_z = vertices.Max(p => p.z);

        var min = new Vector3(min_x, min_y, min_z);
        var max = new Vector3(max_x, max_y, max_z);

        var center = (max + min) * 0.5F;
        var size = max - min;

        return new Bounds ( center, size );
    }


    public static Bounds Bounds ( this IEnumerable<IEnumerable<Vector3>> polygons )
    {
        var polygon = polygons.SelectMany(poly => poly.Select(v=>v));

        return polygon.Bounds ( );
    }

    public static Bounds Bounds ( this List<Vector3> vertices )
    {
        var min_x = vertices.Min(p => p.x);
        var max_x = vertices.Max(p => p.x);
        var min_y = vertices.Min(p => p.y);
        var max_y = vertices.Max(p => p.y);
        var min_z = vertices.Min(p => p.z);
        var max_z = vertices.Max(p => p.z);

        var min = new Vector3(min_x, min_y, min_z);
        var max = new Vector3(max_x, max_y, max_z);

        var center = (max + min) * 0.5F;
        var size = max - min;

        return new Bounds ( center, size );
    }

    public static Bounds Bounds ( this List<List<Vector3>> polygons )
    {
        var polygon = polygons.SelectMany(poly => poly.Select(v=>v));

        return polygon.Bounds ( );
    }

    public static Bounds Bounds ( this Vector3 [ ] vertices )
    {
        var min_x = vertices.Min(p => p.x);
        var max_x = vertices.Max(p => p.x);
        var min_y = vertices.Min(p => p.y);
        var max_y = vertices.Max(p => p.y);
        var min_z = vertices.Min(p => p.z);
        var max_z = vertices.Max(p => p.z);

        var min = new Vector3(min_x, min_y, min_z);
        var max = new Vector3(max_x, max_y, max_z);

        var center = (max + min) * 0.5F;
        var size = max - min;

        return new Bounds ( center, size );
    }

}
