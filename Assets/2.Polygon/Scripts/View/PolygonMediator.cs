using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{
    using strange.extensions.mediation.impl;
    using strange.extensions.dispatcher.eventdispatcher.impl;

    public class PolygonMediator : EventMediator
    {
        [Inject]
        public PolygonView view { set; get; }

        public override void OnRegister ( )
        {
            view.init ( );
        }

        public override void OnRemove ( )
        {

        }
    }

}
