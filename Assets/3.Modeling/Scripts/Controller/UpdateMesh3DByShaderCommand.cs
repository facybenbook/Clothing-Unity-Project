using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Modeling
{

    using strange.extensions.command.impl;

    using strange.extensions.context.api;

    using Polygon;
    using MassSpringSystem;

    using TriangleNet.Geometry;
    using TriangleNet.Meshing;
    using TriangleNet.Smoothing;
    using TriangleNet.Voronoi;
    using TriangleNet.Extensions;

    using System.Linq;

    public class UpdateMesh3DByShaderCommand : EventCommand
    {
        [Inject ( ContextKeys.CONTEXT_VIEW )]
        public GameObject contextView { get; set; }

        [Inject]
        public IEntityLookup polyLookup { set; get; }

        public override void Execute ( )
        {

            Debug.Log ( "Update Mesh 3D by shader." );
            foreach ( var viewEntityPair in polyLookup.libs )
            {
                var polyView = viewEntityPair.Key;

                polyView.pointParent.gameObject.SetActive ( false );
                polyView.lineParent.gameObject.SetActive ( false );
                polyView.meshFilter.gameObject.SetActive ( false );

                var polyEntity = viewEntityPair.Value;

                var triMesh = polyEntity.GetTriangleMesh( 0.3f);

                if ( !polyView.mesh3DHandler )
                {
                    var obj = new GameObject("Mesh 3D Filter");
                    obj.transform.parent = polyView.transform;
                    polyView.mesh3DHandler = obj.AddComponent<ClothingHandler> ( );
                    var meshFilter = obj.AddComponent<MeshFilter>();
                    var renderer = obj.AddComponent<MeshRenderer>();
                    renderer.material = GameObject.Instantiate<Material> ( Resources.Load<Material> ( "Materials/Cloth Material" ) );

                }
                polyView.mesh3DHandler.OnCreate ( triMesh );
            }
        }

        //TriangleNet.Mesh GetTriangleMesh ( IPolygonEntity polyEntity, float segmentLength = 0.2F )
        //{
        //    int count = polyEntity.Points.Count;

        //    var triPolygon = new Polygon();

        //    for ( int i = 0, j = 1; i < polyEntity.Points.Count; i++, j = ( i + 1 ) % count )
        //    {
        //        var curPoint = polyEntity.Points[i];
        //        var nextPoint = polyEntity.Points[j];

        //        var dis = curPoint.DistanceTo(nextPoint);
        //        var num = (int)( dis / segmentLength);

        //        Debug.Log ( "Num : " + num );
        //        int id = i + 1;
        //        var segments = curPoint.SplitToSegments(nextPoint, num).Select(p=>new Vertex(p.x, p.y, id)).ToArray();

        //        for ( int m = 0, n = 1; m < segments.Count ( ) - 1; m++, n = m + 1 )
        //        {
        //            triPolygon.Add ( new Segment ( segments [ m ], segments [ n ], id ), 0 );
        //        }
        //    }

        //    //等边三角形面积公式 ：  S=√3a²/4; √3/4 = 0.443F;
        //    var area = 0.443F * segmentLength * segmentLength ;// * 1.25F;
        //    var options = new ConstraintOptions() { ConformingDelaunay = true };
        //    var quality = new QualityOptions() { MinimumAngle = 30F , MaximumArea = area };// 0.2F };

        //    var triMesh =  (TriangleNet.Mesh)triPolygon.Triangulate(options, quality);

        //    triMesh.Renumber ( );

        //    return triMesh;
        //}
    }

}
