
namespace Utils.PointerHandler
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Collections;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.Serialization;

    using Utils.Extensions;
    public class ClickHandler : MonoBehaviour, IPointerClickHandler
    {

        [Serializable]
        public class ClickedEvent : UnityEvent<Vector3> { }

        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ClickedEvent m_OnClick = new ClickedEvent();

        protected ClickHandler ( ) { }

        public ClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }


        public void OnPointerClick ( PointerEventData data )
        {
            if ( data.button != PointerEventData.InputButton.Left )
                return;

            Vector3 worldPoint;
            if ( !Camera.main.ScreenPointToWorldPointInRectangle ( transform.position, transform.rotation, data.position, out worldPoint ) )
                return;

            Debug.Log ( "Click!" );
            m_OnClick.Invoke ( worldPoint );
        }
    }

}
