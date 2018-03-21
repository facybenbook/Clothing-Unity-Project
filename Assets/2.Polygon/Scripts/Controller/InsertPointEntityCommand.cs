using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{
    using System.Linq;


    using strange.extensions.command.impl;

    using strange.extensions.context.api;
    using strange.extensions.dispatcher.eventdispatcher.api;

    public class InsertPointEntityCommand : EventCommand
    {

        [Inject]
        public IEntityLookup lookup { set; get; }

        [Inject ( "PointPrefab" )]
        public PointView pointPrefab { set; get; }

        [Inject ( "LinePrefab" )]
        public LineView linePrefab { set; get; }

        public override void Execute ( )
        {

            var args = evt.data as InsertAtLineEventArgs;
            var lineView = args.view;
            var pos = args.pos;

            var polyView = lineView.poly;
            var index = polyView.lines.IndexOf(lineView);

            int count = polyView.lines.Count;
            int nextIndex = ( index + 1 ) % count;

            var nextPoint = polyView.points [ nextIndex ];
            
            var polyEntity = lookup.libs[polyView];

            #region Calc 1 new end point ans 2 curves point 's position
            var point_0 = polyEntity.Points[index];
            var point_1 = polyEntity.Points[nextIndex];

            var p0 = point_0.position;
            var p1 = point_1.position;
            var c = point_0.curvePosition;
            var h = Bezier.CalcHandler(p0, p1, c);
            
            var factor = Bezier.BinaryFactor(p0, p1, c, pos);
            
            //var pNew = Bezier.Square(p0, h, p1, factor);
            
            var h0 = Vector3.Lerp(p0, h, factor);
            var h1 = Vector3.Lerp(h, p1, factor);
            var newPointPos = Vector3.Lerp(h0, h1, factor);
            var c0 = Bezier.CalcCurve(p0, newPointPos, h0);
            var c1 = Bezier.CalcCurve(newPointPos, p1, h1);

            #endregion

            polyEntity.Points [ index ].curvePosition = c0;
            //polyEntity.Points [ index ].isCurve = true;

            //var pointEntity = new PointEntity(lineView.centerPosition);
            var pointEntity = new PointEntity(newPointPos, c1);

            var newPoint = pointPrefab.Spawn();
            newPoint.transform.parent = polyView.pointParent;
            newPoint.transform.localPosition = newPointPos;//lineView.centerPosition;

            var newLine = linePrefab.Spawn();
            newLine.transform.parent = polyView.lineParent;
            newLine.transform.localPosition = Vector3.zero;
            newLine.startPoint = newPoint;
            newLine.curvePosition = c1;
            newLine.endPoint = nextPoint;

            lineView.endPoint = newPoint;
            lineView.curvePosition = c0;
            
            newLine.OnRender ( );
            lineView.OnRender ( );

            newPoint.poly = polyView;
            newLine.poly = polyView;


            if ( nextIndex != 0 )
            {
                polyView.points.Insert ( nextIndex, newPoint );
                polyView.lines.Insert ( nextIndex, newLine );
                polyEntity.Points.Insert ( nextIndex, pointEntity );
            }
            else
            {
                polyView.points.Add ( newPoint );
                polyView.lines.Add ( newLine );
                polyEntity.Points.Add ( pointEntity );
            }

            //Debug.Log ( "Insert at " + nextIndex );

        }


    }
}
