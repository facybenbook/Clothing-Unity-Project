using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Modeling
{
    using Polygon;

    public class SewSegment
    {
        
        public PolygonEntity poly { set; get; }
        public PointEntity start { set; get; }
        public PointEntity end { set; get; }

    }

    public class SewSegmentPair
    {
        public SewSegment firstSegment { set; get; }
        public SewSegment secondSegment { set; get; }

    }

    public interface ISewSegmentPairLookup
    {
        List<SewSegmentPair> sewLibs { set; get; }
    }

    public class SewSegmentPairLookup : ISewSegmentPairLookup
    {

        public List<SewSegmentPair> sewLibs { set; get; }

        public SewSegmentPairLookup ( )
        {
            sewLibs = new List<SewSegmentPair> ( );
        }
    }


}