using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{
    using Drawing;
    using strange.extensions.mediation.impl;
    using strange.extensions.dispatcher.eventdispatcher.impl;
    using strange.extensions.dispatcher.eventdispatcher.api;

    public class PointMediator : EventMediator
    {
        
        [Inject]
        public PointView view { set; get; }

        public override void OnRegister ( )
        {
            view.dispatcher.AddListener ( PointView.EVENT.END_DRAG, HandOnUpdate );
            view.dispatcher.AddListener ( PointView.EVENT.DOUBLE_CLICK, HandelOnRemove );
            view.dispatcher.AddListener ( PointView.EVENT.DRAG_OUT, HandelOnRemove );


            dispatcher.AddListener ( Drawing.Event.SELECT_PEN_TOOL, HandleOnDisableCollider );
            dispatcher.AddListener ( Drawing.Event.UNSELECT_PEN_TOOL_REQUEST, HandleOnEnableCollider );

            view.init ( );
        }

        public override void OnRemove ( )
        {
            view.dispatcher.RemoveListener ( PointView.EVENT.END_DRAG, HandOnUpdate );
            view.dispatcher.RemoveListener ( PointView.EVENT.DOUBLE_CLICK, HandelOnRemove );
            view.dispatcher.RemoveListener ( PointView.EVENT.DRAG_OUT, HandelOnRemove );

            dispatcher.RemoveListener ( Drawing.Event.SELECT_PEN_TOOL, HandleOnDisableCollider );
            dispatcher.RemoveListener ( Drawing.Event.UNSELECT_PEN_TOOL_REQUEST, HandleOnEnableCollider );
        }

        private void HandleOnDisableCollider ( IEvent evt )
        {
            view.OnDisableCollider ( );
        }

        private void HandleOnEnableCollider ( IEvent evt )
        {
            view.OnEnableCollider ( );
        }

        void HandOnUpdate ( )
        {
            //var pointEntity = new PointEntity(view.transform.localPosition, view.outLine.curvePosition, view.outLine.IsCurve);
            dispatcher.Dispatch ( Event.UPDATA_POINT_ENTITY, view );
        }

        void HandelOnRemove ( )
        {
            dispatcher.Dispatch ( Event.REMOVE_POINT_ENTITY, view );
        }
    }
}
