using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Polygon
{
    using MassSpringSystem;
    using System.Linq;
    using strange.extensions.mediation.impl;
    using strange.extensions.context.api;

    public class PolygonView : EventView
    {
        
        public List<PointView> points;
        public List<LineView> lines;
        
        public Transform pointParent { set; get; }
        public Transform lineParent { set; get; }

        public MeshFilter meshFilter { set; get; }

        public _GLUtils glUtils { set; get; }

        #region test
        //mass-spring system
        public ClothingHandler mesh3DHandler { set; get; }

        //rigidbody spring physics system
        public SkinnedMeshRenderer forwardSkin { set; get; }
        public SkinnedMeshRenderer backSkin { set; get; }
        #endregion test

        internal void init ( )
        {
        }
        
    }
}
