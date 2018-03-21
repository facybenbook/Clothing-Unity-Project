using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    using Polygon;

    using strange.extensions.context.api;

    public class FreeformPenModel : BasePenModel
    {
                
        private int drawCount { set; get; }
        private Vector3 lastPosition { set; get; }
        
        public override void DrawStart ( Vector3 position)
        {

            OnInit ( );

            CreatePoint ( position );

            lastPosition = position;
            drawCount++;
        }

        public override void DrawTo ( Vector3 position )
        {
            if ( Vector3.Distance ( lastPosition, position ) < 0.4f )
                return;

            if ( drawCount % 2 == 0 )
            {

                CreatePoint ( position );

                int size = pathEntity.Points.Count;

                CreateLine ( size - 2, size - 1 );

            }
            else if ( drawCount % 2 == 1 )
            {
                int size = pathEntity.Points.Count;
                pathEntity.Points [ size - 1 ].curvePosition = position;
            }

            lastPosition = position;
            drawCount++;
        }

        public override void DrawEnd ( )
        {
            if ( drawCount < 6 )
            {
                GameObject.Destroy ( pathView.gameObject );
                pathView = null;
                return;
            }
            
            int size = pathEntity.Points.Count;
            if ( drawCount % 2 == 1 )
            {
                var first = pathEntity.Points[0];
                var last = pathEntity.Points[size - 1];
                last.curvePosition = ( first.position + last.position ) * 0.5f;
            }
            
            CreateLine ( size - 1, 0 );

            lookup.libs.Add ( pathView, pathEntity );

            pathView = null;
            pathEntity = null;
            drawCount = 0;
        }
        
    }
}
