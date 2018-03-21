using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Clothing.Polygon

{

    using TriangleNet.Geometry;
    using TriangleNet.Meshing;
    using TriangleNet.Voronoi;

    using System.Linq;

    using TriangleNet.Topology;
    using dVertex = TriangleNet.Topology.DCEL.Vertex;
    using halfEdge = TriangleNet.Topology.DCEL.HalfEdge;
    
    using strange.extensions.mediation.impl;

    public interface IEntityLookup
    {
        Dictionary<PolygonView, IPolygonEntity> libs { set; get; }
    }

    public class EntityLookup : IEntityLookup
    {
        public Dictionary<PolygonView, IPolygonEntity> libs { set; get; }

        public EntityLookup ( )
        {
            libs = new Dictionary<PolygonView, IPolygonEntity> ( );
        }
    }

    public interface IPolygonEntity
    {
        List<IPointEntity> Points { set; get; }

        IPolygonEntity Clone ( );

    }

    public class PolygonEntity : IPolygonEntity
    {
        public List<IPointEntity> Points { set; get; }

        public PolygonEntity ( )
        {
            Points = new List<IPointEntity> ( );
        }

        public PolygonEntity ( IPointEntity [ ] args ) : this ( )
        {
            Points.AddRange ( args );
        }



        public IPolygonEntity Clone ( )
        {
            var clone = new PolygonEntity();
            foreach ( var point in this.Points )
            {
                clone.Points.Add ( point.Clone ( ) );
            }
            return clone;
        }

    }

    public interface IPointEntity
    {
        Vector3 position { set; get; }
        Vector3 curvePosition { set; get; }

        IPointEntity Clone ( );

        float DistanceTo ( IPointEntity next );
        List<Vector3> SplitToSegments ( IPointEntity next, int count );
    }

    public class PointEntity : IPointEntity
    {

        public Vector3 position { set; get; }
        public Vector3 curvePosition { set; get; }

        public PointEntity ( float x0, float y0, float x1, float y1 ) : this ( new Vector3 ( x0, y0 ), new Vector3 ( x1, y1 ) )
        {
        }

        public PointEntity ( Vector3 pos, Vector3 cur )
        {
            position = pos;
            curvePosition = cur;
        }

        public PointEntity ( Vector3 pos ) : this ( pos, pos )
        {
        }

        public PointEntity ( float x, float y ) : this ( x, y, x, y ) { }

        public IPointEntity Clone ( )
        {
            return new PointEntity ( this.position, this.curvePosition );//, this.isCurve );
        }

        public float DistanceTo ( IPointEntity next )
        {
            return Bezier.ArcLength ( this.position, next.position, this.curvePosition );
        }

        public List<Vector3> SplitToSegments ( IPointEntity next, int count )
        {
            return Bezier.Split ( this.position, next.position, this.curvePosition, count );
        }

        public override string ToString ( )
        {
            return string.Format ( "Pos : {0}, Cur : {1}", position, curvePosition );
        }

    }


    public static class PolygonExtensions
    {
        public static TriangleNet.Mesh GetTriangleMesh ( this IPolygonEntity polyEntity, float segmentLength = 0.1F )
        {
            int count = polyEntity.Points.Count;

            var triPolygon = new Polygon();

            for ( int i = 0, j = 1; i < count; i++, j = ( i + 1 ) % count )
            {
                var curPoint = polyEntity.Points[i];
                var nextPoint = polyEntity.Points[j];

                var dis = curPoint.DistanceTo(nextPoint);

                var num = (int)( dis / segmentLength);

                int id = i + 1;
                var segments = curPoint.SplitToSegments(nextPoint, num).Select(p=>new Vertex(p.x, p.y, id)).ToArray();

                for ( int m = 0, n = 1; m < segments.Count ( ) - 1; m++, n = m + 1 )
                {
                    triPolygon.Add ( new Segment ( segments [ m ], segments [ n ], id ), 0 );
                }
            }

            //等边三角形面积公式 ：  S=√3a²/4; √3/4 = 0.443F;
            var area = 0.443F * segmentLength * segmentLength ;// * 1.25F;
            var options = new ConstraintOptions() { ConformingDelaunay = true };
            var quality = new QualityOptions() { MinimumAngle = 30F , MaximumArea = area };// 0.2F };

            var triMesh =  (TriangleNet.Mesh)triPolygon.Triangulate(options, quality);

            triMesh.Renumber ( );

            return triMesh;
        }

        public static List<List<Vertex>> VonoroiFaces ( this TriangleNet.Mesh triMesh )
        {
            var voronoi = new StandardVoronoi(triMesh);

            List<List<Vertex>> result = new List<List<Vertex>>();

            foreach ( var face in voronoi.Faces )
            {
                result.Add ( face.GetAllVertices ( ).Select ( p => new Vertex ( p.x, p.y, p.id ) ).ToList ( ) );
            }

            var rect = triMesh.Bounds;
            result.Add ( new List<Vertex> ( rect.Path ( ) ) );

            return result;
        }

        /// <summary>
        /// 仅取外轮廓的 关键点 与 曲率点 为 顶点（不再新增三角网格顶点），所构建的网格；
        /// </summary>
        /// <param name="polyEntity"></param>
        /// <returns></returns>
        public static TriangleNet.Mesh BaseTriangleMesh ( this IPolygonEntity polyEntity )
        {
            int count = polyEntity.Points.Count;
            var triPolygon = new Polygon();

            var vertices = new Vertex[count * 2];
            for ( int i = 0; i < count; i++ )
            {
                var point = polyEntity.Points[i];
                vertices [ 2 * i ] = new Vertex ( point.position.x, point.position.y, i );
                vertices [ 2 * i + 1 ] = new Vertex ( point.curvePosition.x, point.curvePosition.y, i );

            }

            var contour = new Contour(vertices);
            triPolygon.Add ( contour );
            return ( TriangleNet.Mesh ) triPolygon.Triangulate ( );
        }

        ///内侧轴线(未完成) ---- 2017-09-07
        ///
        public static void MedialAxis ( this TriangleNet.Mesh triMesh )
        {
            var bounds = triMesh.bounds;
            var voronoi = new StandardVoronoi(triMesh, bounds);

            //List<TriangleNet.Topology.DCEL.HalfEdge> edges = new List<TriangleNet.Topology.DCEL.HalfEdge>();

            Dictionary<dVertex, int> vertLookup = new Dictionary< dVertex, int>();
            foreach ( var vert in voronoi.Vertices.Where ( v => !v.IsOn ( bounds ) ) )
            {
                if ( !vert.leaving.origin.IsOn ( bounds ) )
                {
                    if ( !vertLookup.Keys.Contains ( vert ) )
                        vertLookup.Add ( vert, 0 );
                    vertLookup [ vert ]++;
                }
            }

            foreach ( var vert in vertLookup )
            {

                Debug.Log ( vert.Key + ", " + vert.Value );

            }

            //Dictionary<halfEdge, int> edgeLookup = new Dictionary<halfEdge, int>();
            int count = 0;
            foreach(var edge in voronoi.HalfEdges.Where(e => !e.origin.IsOn ( bounds ) ) )
            {
                Debug.Log (" : " + count ++ + edge + " <=> " + edge.twin );
            }
        }

        public static bool IsOn ( this dVertex vert, Rectangle bounds )
        {
            return new Vertex ( vert.x, vert.y ).IsOn ( bounds );
        }

        public static bool IsOn ( this Vertex vert, Rectangle bounds )
        {

            return vert.IsOn ( bounds.Path ( ) );
            //return false;
        }

        public static bool IsOn ( this Vertex v0, Vertex v1, Vertex v2 )
        {
            return CGAlgorithm.IsOn (
                new Vector2 ( ( float ) v0.x, ( float ) v0.y ),
                new Vector2 ( ( float ) v1.x, ( float ) v1.y ),
                new Vector2 ( ( float ) v2.x, ( float ) v2.y )
                );
        }

        public static bool IsOn ( this Vertex v0, IEnumerable<Vertex> poly )
        {
            var pre = poly.Last();
            foreach ( var vert in poly )
            {
                if ( v0.IsOn ( pre, vert ) )
                    return true;
                pre = vert;
            }
            return false;
        }

        public static Vertex [ ] Path ( this Rectangle triRect )
        {
            return new Vertex [ ]
            {
                new Vertex(triRect.Left, triRect.Bottom),
                new Vertex(triRect.Left, triRect.Top),
                new Vertex(triRect.Right, triRect.Top),
                new Vertex(triRect.Right, triRect.Bottom)
            };
        }

    }
}
