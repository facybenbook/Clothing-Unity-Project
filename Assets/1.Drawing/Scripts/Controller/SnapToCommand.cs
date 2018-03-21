using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    using strange.extensions.command.impl;


    public class SnapToCommand : EventCommand
    {


        [Inject ( "MagneticPenModel" )]
        public IPenModel magneticPenModel { set; get; }

        static int lassoCount = 0;

        public override void Execute ( )
        {
            var position = (Vector3) evt.data;
            magneticPenModel.DrawTo ( position );
            
        }
        
    }
}
