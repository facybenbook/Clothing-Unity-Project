using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    using strange.extensions.command.impl;

    public class SnapStartCommand : EventCommand
    {

        [Inject("MagneticPenModel")]
        public IPenModel magneticPenModel { set; get; }

        public override void Execute ( )
        {
            var position = (Vector3) evt.data;
            magneticPenModel.DrawStart ( position );
        }
    }
}
