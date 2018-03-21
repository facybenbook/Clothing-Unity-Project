namespace Utils.PointerHandler
{

    using System;
    using System.Collections;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;
    using UnityEngine;
    using UnityEngine.UI;

    public class FocusHandler : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler
    {

        [Serializable]
        public class OnEnterEvent : UnityEvent { }
        [FormerlySerializedAs("onEnter")]
        [SerializeField]
        private OnEnterEvent m_OnEnter = new OnEnterEvent();
        public OnEnterEvent onEnter
        {
            get { return m_OnEnter; }
            set { m_OnEnter = value; }
        }

        [Serializable]
        public class OnExitEvent : UnityEvent { }
        [FormerlySerializedAs("onExit")]
        [SerializeField]
        private OnExitEvent m_OnExit = new OnExitEvent();
        public OnExitEvent onExit
        {
            get { return m_OnExit; }
            set { m_OnExit = value; }
        }

        public virtual void OnPointerEnter(PointerEventData data )
        {
            if ( data.dragging)
                return;
                m_OnEnter.Invoke ( );
        }

        public virtual void OnPointerExit(PointerEventData data )
        {
            m_OnExit.Invoke ( );
        }
    }
}
