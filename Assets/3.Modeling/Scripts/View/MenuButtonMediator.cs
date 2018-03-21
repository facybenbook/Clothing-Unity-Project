using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Modeling

{

    using strange.extensions.mediation.impl;
    using strange.extensions.dispatcher.eventdispatcher.api;

    public class MenuButtonMediator : EventMediator
    {

        [Inject]
        public MenuButtonView view { set; get; }

        public override void OnRegister ( )
        {
            view.dispatcher.AddListener ( MenuButtonView.CLICK_EVENT, HandleOnMenuClicked );
            view.init ( );
        }

        public override void OnRemove ( )
        {
            view.dispatcher.RemoveListener ( MenuButtonView.CLICK_EVENT, HandleOnMenuClicked );
        }

        void HandleOnMenuClicked(IEvent evt )
        {
            var name = (MenuName)evt.data;
            switch ( name )
            {

                case MenuName.STUFF:
                    dispatcher.Dispatch ( Event.STUFF );
                    break;

                case MenuName.EMPTY:
                    dispatcher.Dispatch ( Event.EMPTY );
                    break;

                case MenuName.COMPUTE_SHADER:
                    dispatcher.Dispatch ( Event.COMPUTER_SHADER );
                    break;

                case MenuName.SIMPLE_SEWING:
                    dispatcher.Dispatch ( Event.SIMPLE_SEW );
                    break;

                case MenuName.TO_EDIT:
                    dispatcher.Dispatch ( Event.TO_EDIT );
                    break;
                default:
                    break;
            }

        }

    }
}
