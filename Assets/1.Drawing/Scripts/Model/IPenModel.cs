using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{


    using Polygon;


    using strange.extensions.context.api;

    public interface IPenModel
    {
        void DrawStart ( Vector3 position );
        void DrawTo ( Vector3 position );
        void DrawEnd ( );
    }

    public abstract class BasePenModel : IPenModel
    {
        [Inject ( ContextKeys.CONTEXT_VIEW )]
        public GameObject contextView { get; set; }
        
        [Inject]
        public IEntityLookup lookup { set; get; }

        [Inject ( "PointPrefab" )]
        public PointView pointPrefab { set; get; }

        [Inject ( "LinePrefab" )]
        public LineView linePrefab { set; get; }

        protected PolygonView pathView { set; get; }
        protected PolygonEntity pathEntity { set; get; }

        public abstract void DrawStart ( Vector3 position );
        public abstract void DrawTo ( Vector3 position );
        public abstract void DrawEnd ( );

        protected void CreatePoint ( Vector3 position )
        {

            IPointEntity point = new PointEntity(position);
            pathEntity.Points.Add ( point );

            var pointView = pointPrefab.Spawn(position);
            pointView.transform.parent = pathView.pointParent;

            pathView.points.Add ( pointView );
            pointView.poly = pathView;

        }

        protected void CreateLine ( int startIndex, int endIndex )
        {
            var prePointView = pathView.points[startIndex];
            var curPointView = pathView.points[endIndex];

            var lineView = linePrefab.Spawn();
            lineView.transform.parent = pathView.lineParent;
            lineView.startPoint = prePointView;
            lineView.endPoint = curPointView;
            lineView.curvePosition = pathEntity.Points [ startIndex ].curvePosition;
            lineView.OnRender ( );

            pathView.lines.Add ( lineView );
            lineView.poly = pathView;
        }

        protected void OnInit ( )
        {
            var polyObj = new GameObject();
            var polyView = polyObj.AddComponent<PolygonView>();
            polyObj.transform.parent = contextView.transform;
            polyObj.name = "Drawing Path";

            polyView.glUtils = polyObj.AddComponent<_GLUtils>();
            

            var lineParentObj = new GameObject("LineGroup");
            lineParentObj.transform.parent = polyObj.transform;
            polyView.lineParent = lineParentObj.transform;

            var pointParentObj = new GameObject("PointGroup");
            pointParentObj.transform.parent = polyObj.transform;
            polyView.pointParent = pointParentObj.transform;

            polyView.points = new List<PointView> ( );
            polyView.lines = new List<LineView> ( );

            pathView = polyView;
            pathEntity = new PolygonEntity ( );
        }
    }
}

