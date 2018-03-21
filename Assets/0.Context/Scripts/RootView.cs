using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Context
{

    using strange.extensions.context.impl;

    public class RootView : ContextView
    {
        public RectTransform effectiveArea;
        
        public AnimationCurve stuffCurve;
        public AnimationCurve emptyCurve;

        //public Drawing.Context drawingContext;

        void Awake ( )
        {
            Debug.Log ( "Context is awake!" );
            context = new RootContext ( this );

        }

    }

}