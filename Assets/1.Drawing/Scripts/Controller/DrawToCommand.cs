using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    using Polygon;
    using strange.extensions.command.impl;

    using strange.extensions.context.api;

    public class DrawToCommand : EventCommand
    {
        [Inject ( "FreeformPenModel" )]
        public IPenModel freeformPenModel { set; get; }
        
        public override void Execute ( )
        {
            
            var position = (Vector3) evt.data;
            freeformPenModel.DrawTo ( position );
            
        }
        
    }
}
