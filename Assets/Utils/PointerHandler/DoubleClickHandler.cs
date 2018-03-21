
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

    public class DoubleClickHandler : MonoBehaviour, IPointerClickHandler
    {

        [Serializable]
        public class DoubleClickedEvent : UnityEvent<Vector3> { }
        
        [FormerlySerializedAs("onDoubleClick")]
        [SerializeField]
        private DoubleClickedEvent m_OnDoubleClick = new DoubleClickedEvent();

        protected DoubleClickHandler ( ) { }

        public DoubleClickedEvent onDoubleClick
        {
            get { return m_OnDoubleClick; }
            set { m_OnDoubleClick = value; }
        }
        
        #region Click

        public void OnPointerClick ( PointerEventData data )
        {
            if ( data.button != PointerEventData.InputButton.Left  )
            {
                return;
            }

            if ( !hasClickedOnce )
            {
                StartCoroutine ( Timer ( ) );
            }
            else
            {
                HandleOnDoubleClick ( data );
            }
        }

        void HandleOnDoubleClick ( PointerEventData data )
        {
            Vector3 worldPoint;
            if ( !Camera.main.ScreenPointToWorldPointInRectangle ( transform.position, transform.rotation, data.position, out worldPoint ) )
                return;

            Debug.Log ( "Double Click!" );
            hasClickedOnce = false;
            m_OnDoubleClick.Invoke ( worldPoint );
        }

        float duration = 0.5F;
        float pastTime = 0F;
        bool hasClickedOnce = false;
        //bool hasClickedTwice = false;
        IEnumerator Timer ( )
        {
            //Debug.Log ( "Single Click!" );
            pastTime = 0F;
            hasClickedOnce = true;
            while (hasClickedOnce && pastTime < duration )
            {
                pastTime += Time.deltaTime;
                yield return new WaitForFixedUpdate ( );
            }
            
            hasClickedOnce = false;
            yield return null;
        }
        #endregion
        
    }

}