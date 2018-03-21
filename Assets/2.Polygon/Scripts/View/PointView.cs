using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

namespace Clothing.Polygon
{
    using strange.extensions.mediation.impl;

    public class PointView : EventView
    {

        public enum EVENT { DRAG_OUT, END_DRAG, DOUBLE_CLICK }

        public PolygonView poly { set; get; }

        public LineView inLine { set; get; }
        public LineView outLine { set; get; }

        internal void init ( )
        {
            OnDisableCollider ( );
        }

        public void HandleOnDoubleClick ( Vector3 worldPosition )
        {
            dispatcher.Dispatch ( EVENT.DOUBLE_CLICK );
        }

        public void OnRecycle ( )
        {
            inLine = null;
            outLine = null;
            poly = null;
            this.Recycle ( );
        }

        #region Collider
        public void OnEnableCollider ( )
        {
            GetComponent<Collider> ( ).enabled = true;
        }

        public void OnDisableCollider ( )
        {

            GetComponent<Collider> ( ).enabled = false;
        }
        #endregion Collider

        #region Drag

        private Vector3 deltaPos = Vector3.zero;
        private Vector3 beginPos;

        public void HandleOnBeginDrag ( Vector3 worldPosition )
        {
            beginPos = transform.position;
            deltaPos = transform.position - worldPosition;

            HandleOnGotFocus ( );
        }

        public void HandleOnDrag ( Vector3 worldPosition )
        {
            var castPos = worldPosition + deltaPos;
            transform.position = new Vector3 ( castPos.x, castPos.y, transform.position.z );

            inLine.OnRender ( );
            outLine.OnRender ( );

            HandleOnGotFocus ( );

        }


        public void HandleOnDragOut ( )
        {
            dispatcher.Dispatch ( EVENT.DRAG_OUT );
        }

        public void HandleOnEndDrag ( )
        {

            HandleOnLostFocus ( );
            dispatcher.Dispatch ( EVENT.END_DRAG );
        }


        #endregion Drag
        
        public void HandleOnGotFocus ( )
        {
            gameObject.GetComponent<MeshRenderer> ( ).material.color = Color.red;
        }

        public void HandleOnLostFocus ( )
        {
            gameObject.GetComponent<MeshRenderer> ( ).material.color = Color.white;
        }
    }
}
