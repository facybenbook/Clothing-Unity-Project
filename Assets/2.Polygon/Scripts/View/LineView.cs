using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Clothing.Polygon
{
    using strange.extensions.mediation.impl;

    public class LineView : EventView
    {

        public enum EVENT { END_DRAG, DOUBLE_CLICK, DRAG_OUT}

        public PolygonView poly { set; get; }
        
        //public bool IsCurve { set; get; }

        public Vector3 curvePosition { set; get; }

        private Vector3 startPosition { get { return startPoint.transform.localPosition; } }
        private Vector3 endPosition { get { return endPoint.transform.localPosition; } }

        private PointView _startPoint;
        public PointView startPoint { set { _startPoint = value; _startPoint.outLine = this; } get { return _startPoint; } }
        private PointView _endPoint;
        public PointView endPoint { set { _endPoint = value; _endPoint.inLine = this; } get { return _endPoint; } }

        internal void init ( )
        {
            OnDisableCollider ( );
        }   
        
        public void OnRender ( )
        {
            if ( !startPoint || !endPoint ) return;
            var render = GetComponent<LineMeshBuilder> ( );//.OnRender ( );
            render.startPosition = startPosition;
            render.endPosition = endPosition;
            //render.IsCurve = true;
            render.curvePosition = curvePosition;
            render.OnRender ( );
        }

        public void HandleOnDoubleClick ( Vector3 position )
        {
            //var centerPos = GetComponent<LineMeshBuilder>().centerPosition;
            //poly.Split ( this, centerPos );
            dispatcher.Dispatch ( EVENT.DOUBLE_CLICK, position );
        }

        public void OnRecycle ( )
        {
            _startPoint = null;
            _endPoint = null;
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

        public void HandleOnBeginDrag ( Vector3 worldPosition )
        {
            //if ( !IsCurve ) { IsCurve = true; curvePosition = worldPosition; }

            deltaPos = curvePosition - worldPosition;
            HandleOnGotFocus ( );
        }

        public void HandleOnDrag ( Vector3 worldPosition )
        {
            var castPos = worldPosition + deltaPos;
            curvePosition = new Vector3 ( castPos.x, castPos.y, 0 );
            HandleOnGotFocus ( );
            OnRender ( );
        }


        public void HandleOnDragOut ( )
        {
            //IsCurve = false;
            curvePosition = ( startPosition + endPosition ) * 0.5f;
            OnRender ( );
            HandleOnLostFocus ( );
            dispatcher.Dispatch ( EVENT.DRAG_OUT );
        }

        public void HandleOnEndDrag ( )
        {

            HandleOnLostFocus ( );
            dispatcher.Dispatch ( EVENT.END_DRAG );
        }

        #endregion Drag

        #region Focus

        public void HandleOnGotFocus ( )
        {
            gameObject.GetComponent<MeshRenderer> ( ).material.color = Color.red;
        }

        public void HandleOnLostFocus ( )
        {
            gameObject.GetComponent<MeshRenderer> ( ).material.color = Color.white;
        }
        #endregion



    }
}
