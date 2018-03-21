using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{

    public class EventArgs
    {

        public PolygonView poly { set; get; }
        public PointView point { set; get; }

    }

    public class InsertAtLineEventArgs
    {
        public LineView view { set; get; }
        public Vector3 pos { set; get; }
    }
}
