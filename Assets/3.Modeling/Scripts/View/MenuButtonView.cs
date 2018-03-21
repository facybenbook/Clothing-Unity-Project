using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Modeling

{

    using UnityEngine.UI;
    using strange.extensions.mediation.impl;
    using strange.extensions.dispatcher.eventdispatcher.impl;
    public class MenuButtonView : EventView
    {

        internal const string CLICK_EVENT = "CLICK_EVENT";

        public MenuName name;
        
        internal void init ( )
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener ( HandleOnClick );
        }


        void HandleOnClick ( )
        {
            dispatcher.Dispatch ( CLICK_EVENT, name );
        }

    }
}
