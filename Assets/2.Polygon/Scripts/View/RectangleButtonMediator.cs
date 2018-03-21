using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{

    using strange.extensions.dispatcher.eventdispatcher.api;
    using strange.extensions.mediation.impl;

    public class RectangleButtonMediator : EventMediator
    {
        [Inject]
        public RectangleButtonView view { set; get;}

        public override void OnRegister ( )
        {
            view.dispatcher.AddListener ( RectangleButtonView.CLICK_EVENT, HandleOnViewClick );
            view.init ( );
        }

        public override void OnRemove ( )
        {
            view.dispatcher.RemoveListener ( RectangleButtonView.CLICK_EVENT, HandleOnViewClick );
        }

        private void HandleOnViewClick(IEvent evt)
        {
            var type = (PrimitiveType)evt.data;
            dispatcher.Dispatch ( Event.GENERATE_PRIMITIVE, type );
        }
    }
}
