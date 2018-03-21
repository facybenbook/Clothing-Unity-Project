using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{

    using strange.extensions.mediation.impl;
    using strange.extensions.dispatcher.eventdispatcher.api;

    public class CanvasMediator : EventMediator
    {

        [Inject]
        public CanvasView view { set; get; }


        public override void OnRegister ( )
        {

            view.dispatcher.AddListener ( CanvasView.EVENT.DRAG_START, HandleOnDragStart );
            view.dispatcher.AddListener ( CanvasView.EVENT.DRAG_TO, HandleOnDragTo );
            view.dispatcher.AddListener ( CanvasView.EVENT.DRAG_END, HandleOnDragEnd );

            dispatcher.AddListener ( Event.SELECT_PEN_TOOL, HandleOnEnableDraw );
            dispatcher.AddListener ( Event.UNSELECT_PEN_TOOL_REQUEST, HandleOnDisableDraw );
            
            view.init ( );
        }

        public override void OnRemove ( )
        {
            view.dispatcher.RemoveListener ( CanvasView.EVENT.DRAG_START, HandleOnDragStart );
            view.dispatcher.RemoveListener ( CanvasView.EVENT.DRAG_TO, HandleOnDragTo );
            view.dispatcher.RemoveListener ( CanvasView.EVENT.DRAG_END, HandleOnDragEnd );


            dispatcher.RemoveListener ( Event.SELECT_PEN_TOOL, HandleOnEnableDraw );
            dispatcher.RemoveListener ( Event.UNSELECT_PEN_TOOL_REQUEST, HandleOnDisableDraw );
        }

        private void HandleOnDragStart(IEvent evt )
        {
            var position = (Vector3) evt.data;
            switch ( view.Pen )
            {
                case PenType.FREEFORM_PEN:

                    dispatcher.Dispatch ( Event.DRAW_START, position );
                    break;
                case PenType.MAGNETIC_PEN:
                    dispatcher.Dispatch ( Event.SNAP_START, position );
                    break;
                default:
                    break;
            }
        }

        private void HandleOnDragTo ( IEvent evt )
        {
            var position = (Vector3) evt.data;
            switch ( view.Pen )
            {
                case PenType.FREEFORM_PEN:

                    dispatcher.Dispatch ( Event.DRAW_TO, position );
                    break;
                case PenType.MAGNETIC_PEN:
                    dispatcher.Dispatch ( Event.SNAP_TO, position );
                    break;
                default:
                    break;
            }
        }

        private void HandleOnDragEnd ( )
        {
            switch ( view.Pen )
            {
                case PenType.FREEFORM_PEN:

                    dispatcher.Dispatch ( Event.DRAW_END );
                    break;
                case PenType.MAGNETIC_PEN:
                    dispatcher.Dispatch ( Event.SNAP_END );
                    break;
                default:
                    break;
            }
        }

        private void HandleOnEnableDraw ( IEvent evt )
        {
            var pen = ( PenType ) evt.data;
            if(view.Pen != PenType.UNKNOWN )
            {
                dispatcher.Dispatch ( Event.UNSELECT_PEN_TOOL_RESPONSE, view.Pen );
            }
            view.OnEnableCollider ( pen );
        }

        private void HandleOnDisableDraw ( IEvent evt )
        {
            var pen = ( PenType ) evt.data;
            if ( view.Pen != pen ) return;
            view.OnDisableCollider ( );
        }
    }
}
