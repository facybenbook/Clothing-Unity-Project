using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    using strange.extensions.command.impl;
    public class SnapEndCommand : EventCommand
    {


        [Inject ( "MagneticPenModel" )]
        public IPenModel magneticPenModel { set; get; }

        public override void Execute ( )
        {
            magneticPenModel.DrawEnd ( );
        }
    }
}
