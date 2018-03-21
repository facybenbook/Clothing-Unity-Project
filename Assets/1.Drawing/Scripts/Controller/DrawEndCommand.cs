using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    using Polygon;
    using strange.extensions.command.impl;
    public class DrawEndCommand : EventCommand
    {
        
        [Inject ( "FreeformPenModel" )]
        public IPenModel freeformPenModel { set; get; }
        
        public override void Execute ( )
        {
            freeformPenModel.DrawEnd ( );            
        }
        
    }

}