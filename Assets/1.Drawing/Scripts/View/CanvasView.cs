using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    using UnityEngine.UI;

    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using Utils.PointerHandler;
    using strange.extensions.mediation.impl;

    public class CanvasView : EventView
    {
        
        public enum EVENT
        {
            DRAG_START,
            DRAG_TO,
            DRAG_END
        }

        public PenType Pen { set; get; }

        internal void init ( )
        {
            //OnDisableCollider ( );
            GetComponent<DragHandler> ( ).NeedsDragOut = false;
        }

        public void OnEnableCollider ( PenType _pen )
        {
            Pen = _pen;
            //GetComponent<MeshCollider> ( ).enabled = true;
        }

        public void OnDisableCollider ( )
        {
            Pen = PenType.UNKNOWN;
            //GetComponent<MeshCollider> ( ).enabled = false;
        }

        bool isDraging = false;
        Vector3 dragPosition;
        float Duration = 0.1f;
        private IEnumerator WaitForDragEnd ( )
        {
            yield return new WaitForSeconds ( Duration );
            while ( isDraging )
            {
                dispatcher.Dispatch ( EVENT.DRAG_TO, dragPosition );
                yield return new WaitForSeconds ( Duration );
            }
            dispatcher.Dispatch ( EVENT.DRAG_END );
            yield return null;

        }
        
        #region Drag
        public void HandleOnBeginDrag ( Vector3 position )
        {
            isDraging = true;
            dragPosition = position;

            dispatcher.Dispatch ( EVENT.DRAG_START, dragPosition );

            StartCoroutine ( WaitForDragEnd ( ) );
        }

        public void HandleOnDrag ( Vector3 position )
        {
            dragPosition = position;
        }

        public void HandleOnDragOut ( )
        {
            //isDraging = false;
        }

        public void HandleOnEndDrag ( )
        {
            isDraging = false;
        }

        #endregion Drag
    }
}
