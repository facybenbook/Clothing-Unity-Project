using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Modeling
{
    using strange.extensions.command.impl;

    using Polygon;

    public class ToEditCommand : EventCommand
    {

        [Inject]
        public IEntityLookup polyLookup { set; get; }

        public override void Execute ( )
        {
            Debug.Log ( "To Edit." );
            foreach ( var viewEntityPair in polyLookup.libs )
            {

                var polyView = viewEntityPair.Key;

                polyView.pointParent.gameObject.SetActive ( true );
                polyView.lineParent.gameObject.SetActive ( true );
                polyView.meshFilter.gameObject.SetActive ( true );

                if ( polyView.forwardSkin ) GameObject.Destroy ( polyView.forwardSkin.gameObject );
                if ( polyView.backSkin ) GameObject.Destroy ( polyView.backSkin.gameObject );



            }
        }
    }
}
