using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{
    using System.Linq;

    using strange.extensions.command.impl;

    using strange.extensions.context.api;
    using strange.extensions.dispatcher.eventdispatcher.api;

    public class CreatePrimitiveCommand : EventCommand
    {

        [Inject ( ContextKeys.CONTEXT_VIEW )]
        public GameObject contextView { get; set; }

        [Inject]
        public IEntityLookup lookup { set; get; }
        
        [Inject ( "PointPrefab" )]
        public PointView pointPrefab { set; get; }

        [Inject ( "LinePrefab" )]
        public LineView linePrefab { set; get; }

        static int polyCount = 0;

        public override void Execute ( )
        {

            var type = (PrimitiveType) evt.data;
            Debug.Log ( "Create primitive! " + type );

            var polyEntity = PrimitiveCreator.Create ( type );
            
            Onbuild ( polyEntity );
            
        }

        void Onbuild (IPolygonEntity polyEntity )
        {

            var polyObj = new GameObject();
            var polyView = polyObj.AddComponent<PolygonView>();
            polyObj.transform.parent = contextView.transform;
            polyObj.name = "Polygon_" + polyCount++;

            var lineParentObj = new GameObject("LineGroup");
            lineParentObj.transform.parent = polyObj.transform;
            polyView.lineParent = lineParentObj.transform;

            var pointParentObj = new GameObject("PointGroup");
            pointParentObj.transform.parent = polyObj.transform;
            polyView.pointParent = pointParentObj.transform;
            
            int count = polyEntity.Points.Count;

            polyView.points = new List<PointView> ( );
            polyView.lines = new List<LineView> ( );

            var pointViews = polyEntity.Points.Select( p => pointPrefab.Spawn(p.position)).ToArray();

            for ( int i = 0, j = 1; i < count; i++, j = ( i + 1 ) % count )
            {
                var entity = polyEntity.Points[i];

                var pointView = pointViews[i];
                pointView.transform.parent = polyView.pointParent;

                var lineView = linePrefab.Spawn();
                lineView.transform.parent = polyView.lineParent;
                lineView.startPoint = pointViews [ i ];
                lineView.endPoint = pointViews [ j ];
                lineView.curvePosition = entity.curvePosition;
                lineView.OnRender ( );

                polyView.points.Add ( pointView );
                pointView.poly = polyView;

                polyView.lines.Add ( lineView );
                lineView.poly = polyView;
            }
            
            lookup.libs.Add ( polyView, polyEntity );
           
        }

    }
}
