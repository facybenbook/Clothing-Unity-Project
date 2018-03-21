using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    using strange.extensions.command.impl;

    public class DrawStartCommand : EventCommand
    {

        [Inject ( "FreeformPenModel" )]
        public IPenModel freeformPenModel { set; get; }

        public override void Execute ( )
        {
            var position = (Vector3) evt.data;
            freeformPenModel.DrawStart ( position );
        }
    }
}
