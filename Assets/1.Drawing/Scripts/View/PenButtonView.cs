using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Clothing.Drawing
{
    using UnityEngine.UI;
    using strange.extensions.mediation.impl;
    public class PenButtonView : EventView
    {

        public PenType type;

        internal enum EVENT { SELECTED, UNSELECTED }

        private Image _img;
        private Image img { get { if ( !_img ) _img = GetComponent<Image> ( ); return _img; } }

        internal void init ( )
        {
            isSelected = false;
            img.color = Color.white;
        }

        #region Click
        bool isSelected = false;
        public void HandleOnClick ( Vector3 worldPos )
        {

            if ( isSelected )
                HandleOnUnselected ( );
            else
                HandleOnSelected ( );
        }
        
        public void HandleOnSelected ( )
        {
            if ( isSelected ) return;

            isSelected = true;
            img.color = Color.magenta;

            dispatcher.Dispatch ( EVENT.SELECTED );
        }

        public void HandleOnUnselected ( )
        {
            if ( !isSelected ) return;

            isSelected = false;
            img.color = Color.white;

            dispatcher.Dispatch ( EVENT.UNSELECTED );
        }

        #endregion

        #region Focus

        public void HandleOnEnter ( )
        {
            if ( !isSelected )
                img.color = Color.yellow;
        }

        public void HandleOnExit ( )
        {
            if ( !isSelected )
                img.color = Color.white;
        }

        #endregion
    }
}
