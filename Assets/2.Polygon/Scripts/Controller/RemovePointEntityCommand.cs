using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{
    using System.Linq;

    using strange.extensions.command.impl;

    using strange.extensions.context.api;
    using strange.extensions.dispatcher.eventdispatcher.api;

    public class RemovePointEntityCommand : EventCommand
    {

        [Inject]
        public IEntityLookup lookup { set; get; }

        public override void Execute ( )
        {

            var pointView = evt.data as PointView;
            var polyView = pointView.poly;
            var polyEntity = lookup.libs[polyView];

            int count = polyEntity.Points.Count;
            if ( count <= 3 ) return;

            var index = polyView.points.IndexOf(pointView);
            var preIndex = ( count + index - 1) % count;
            var nextIndex = (index + 1) % count;


            //Calc new curve position
            var point_0 = polyEntity.Points[preIndex];
            var point_1 = polyEntity.Points[index];
            var point_2 = polyEntity.Points[nextIndex];

            var p0 = point_0.position;
            var p1 = point_1.position;
            var p2 = point_2.position;

            var h0 = Bezier.CalcHandler(p0, p1, point_0.curvePosition);
            var h1 = Bezier.CalcHandler(p1, p2, point_1.curvePosition);

            var newCurvePos = (h0 + h1) * 0.5f;

            Vector2 skew = (Vector2) newCurvePos;
            if ( CGAlgorithm.Intersect2D_2SkewSegments ( p0, h0, p2, h1, ref skew ) )
            {
                var cIsLeft = Mathf.Sign(CGAlgorithm.IsLeft(p0, p2, p1));
                var sIsLeft = Mathf.Sign(CGAlgorithm.IsLeft(p0, p2, skew));
                if ( cIsLeft * sIsLeft > 0 )
                {
                    newCurvePos = Bezier.CalcCurve ( p0, p2, ( Vector3 ) skew );
                }
            }


            var pointEntity = polyEntity.Points[index];
            var lineView = polyView.lines[index];
            var preLine = polyView.lines[preIndex];
            var nextPoint = polyView.points[nextIndex];

            preLine.endPoint = nextPoint;
            preLine.curvePosition = newCurvePos;// pointEntity.position;
            preLine.OnRender ( );

            pointView.OnRecycle ( );
            lineView.OnRecycle ( );

            polyView.points.Remove ( pointView );
            polyView.lines.Remove ( lineView );

            //polyEntity.Points [ preIndex ].isCurve = true;
            polyEntity.Points [ preIndex ].curvePosition = newCurvePos;// pointEntity.position;

            polyEntity.Points.RemoveAt ( index );

        }
    }
}
