namespace Utils.PointerHandler

{
    using System;
    using System.Collections;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using UnityEngine;
    using UnityEngine.UI;

    using strange.extensions.mediation.impl;

    using Utils.Extensions;



    public class DragHandler : View, IBeginDragHandler, IDragHandler, IEndDragHandler//, IMoveHandler
    {

        //Effective area
        [Inject ( "EffectiveArea" )]
        public RectTransform effectiveArea { set; get; }

        private bool isDragging = false;
        public bool NeedsDragOut = true;

        [Serializable]
        public class BeginDragtEvent : UnityEvent<Vector3> { }
        [FormerlySerializedAs("onBeginDrag")]
        [SerializeField]
        private BeginDragtEvent m_OnBeginDrag = new BeginDragtEvent();
        public BeginDragtEvent onBeginDrag
        {
            get { return m_OnBeginDrag; }
            set { m_OnBeginDrag = value; }
        }

        [Serializable]
        public class DragEvent : UnityEvent<Vector3> { }
        [FormerlySerializedAs("onDrag")]
        [SerializeField]
        private DragEvent m_OnDrag = new DragEvent();
        public DragEvent onDrag
        {
            get { return m_OnDrag; }
            set { m_OnDrag = value; }
        }

        [Serializable]
        public class DragOutEvent : UnityEvent { }
        [FormerlySerializedAs("onDragOut")]
        [SerializeField]
        private DragOutEvent m_OnDragOut = new DragOutEvent();
        public DragOutEvent onDragOut
        {
            get { return m_OnDragOut; }
            set { m_OnDragOut = value; }
        }

        [Serializable]
        public class EndDragEvent : UnityEvent { }
        [FormerlySerializedAs("onEndDrag")]
        [SerializeField]
        private EndDragEvent m_OnEndDrag = new EndDragEvent();
        public EndDragEvent onEndDrag
        {
            get { return m_OnEndDrag; }
            set { m_OnEndDrag = value; }
        }

        //Vector3 RaycastPosition;

        public virtual void OnBeginDrag ( PointerEventData data )
        {
            if ( data.button != PointerEventData.InputButton.Left )
                return;

            Vector3 worldPoint;
            if ( !Camera.main.ScreenPointToWorldPointInRectangle ( transform.position, transform.rotation, data.position, out worldPoint ) )
                return;
            
            isDragging = true;
            if ( m_OnBeginDrag != null )
                m_OnBeginDrag.Invoke ( worldPoint ); //( data.pointerCurrentRaycast.worldPosition );
        }

        public virtual void OnDrag ( PointerEventData data )
        {

            if ( data.button != PointerEventData.InputButton.Left )
                return;

            if ( !isDragging ) return;

            Vector3 worldPoint;
            if ( !Camera.main.ScreenPointToWorldPointInRectangle ( transform.position, transform.rotation, data.position, out worldPoint ) )
                return;

            m_OnDrag.Invoke ( worldPoint );

            if ( NeedsDragOut && !RectTransformUtility.RectangleContainsScreenPoint ( effectiveArea, data.position ) )
            {
                //Debug.Log ( "Handle Drag Out!" );
                if ( m_OnDragOut != null )
                    m_OnDragOut.Invoke ( );

                isDragging = false;
            }
        }

        public virtual void OnEndDrag ( PointerEventData data )
        {
            if ( data.button != PointerEventData.InputButton.Left )
                return;

            isDragging = false;
            if ( m_OnEndDrag != null )
                m_OnEndDrag.Invoke ( );
        }

    }

}
