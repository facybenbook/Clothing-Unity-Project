using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{

    public enum Event
    {
        SELECT_PEN_TOOL,
        UNSELECT_PEN_TOOL_REQUEST,
        UNSELECT_PEN_TOOL_RESPONSE,

        DRAW_START,
        DRAW_TO,
        DRAW_END,

        SNAP_START,
        SNAP_TO,
        SNAP_END
        

    }


}
