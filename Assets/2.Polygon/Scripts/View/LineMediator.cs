using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{

    using strange.extensions.mediation.impl;
    using strange.extensions.dispatcher.eventdispatcher.api;
    public class LineMediator : EventMediator
    {

        [Inject]
        public LineView view { set; get; }

        public override void OnRegister ( )
        {
            view.dispatcher.AddListener ( LineView.EVENT.END_DRAG, HandleOnUpdate );
            view.dispatcher.AddListener ( LineView.EVENT.DOUBLE_CLICK, HandleOnInsert );
            view.dispatcher.AddListener ( LineView.EVENT.DRAG_OUT, HandleOnUpdate );


            dispatcher.AddListener ( Drawing.Event.SELECT_PEN_TOOL, HandleOnDisableCollider );
            dispatcher.AddListener ( Drawing.Event.UNSELECT_PEN_TOOL_REQUEST, HandleOnEnableCollider );

            view.init ( );
        }

        public override void OnRemove ( )
        {
            view.dispatcher.RemoveListener ( LineView.EVENT.END_DRAG, HandleOnUpdate );
            view.dispatcher.RemoveListener ( LineView.EVENT.DOUBLE_CLICK, HandleOnInsert );
            view.dispatcher.RemoveListener ( LineView.EVENT.DRAG_OUT, HandleOnUpdate );


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

        void HandleOnUpdate ( )
        {
            dispatcher.Dispatch ( Event.UPDATA_POINT_ENTITY, view.startPoint );
        }

        void HandleOnInsert ( IEvent evt )
        {
            var worldPos = (Vector3) evt.data;
            var pos = view.transform.InverseTransformPoint(worldPos);
            var args = new InsertAtLineEventArgs() {view = view, pos = pos };
            dispatcher.Dispatch ( Event.INSERT_POINT_ENTITY, args );
        }
    }
}
