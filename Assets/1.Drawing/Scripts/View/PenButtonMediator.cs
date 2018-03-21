using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    using strange.extensions.mediation.impl;
    using strange.extensions.dispatcher.eventdispatcher.api;
    public class PenButtonMediator : EventMediator
    {

        [Inject]
        public PenButtonView view { set; get; }

        public override void OnRegister ( )
        {

            view.dispatcher.AddListener ( PenButtonView.EVENT.SELECTED, HandleOnSelected);
            view.dispatcher.AddListener ( PenButtonView.EVENT.UNSELECTED, HandleOnUnselected );

            dispatcher.AddListener ( Event.UNSELECT_PEN_TOOL_RESPONSE, ResponseToUnselect );

            view.init ( );

        }

        public override void OnRemove ( )
        {

            view.dispatcher.RemoveListener ( PenButtonView.EVENT.SELECTED, HandleOnSelected );
            view.dispatcher.RemoveListener ( PenButtonView.EVENT.UNSELECTED, HandleOnUnselected );


            dispatcher.RemoveListener ( Event.UNSELECT_PEN_TOOL_RESPONSE, ResponseToUnselect );
        }

        private void HandleOnSelected ( IEvent evt )
        {
            dispatcher.Dispatch ( Event.SELECT_PEN_TOOL, view.type);
        }

        private void HandleOnUnselected(IEvent evt )
        {
            dispatcher.Dispatch ( Event.UNSELECT_PEN_TOOL_REQUEST, view.type );
        }

        public void ResponseToUnselect(IEvent evt )
        {
            Debug.Log ( "ResponseToUnselec" );
            var type = (PenType) evt.data;
            if ( type != view.type ) return;
            view.HandleOnUnselected ( );
        }

    }

}