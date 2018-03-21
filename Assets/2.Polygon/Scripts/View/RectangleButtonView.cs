using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{

    using UnityEngine.UI;

    using strange.extensions.mediation.impl;
    using strange.extensions.dispatcher.eventdispatcher.api;

    public class RectangleButtonView : EventView
    {
        internal const string CLICK_EVENT = "CLICK_EVENT";
        public PrimitiveType type;

        internal void init ( )
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener ( HandleOnClick );
        }


        void HandleOnClick ( )
        {
            dispatcher.Dispatch ( CLICK_EVENT, type );
        }
    }
}
