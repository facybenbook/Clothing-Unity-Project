using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Clothing.Polygon
{
    public class PointMeshBuilder : _Square
    {

        public float size = 0.2F;

        protected override void HandleOnUpdateCollider ( )
        {
            var collider = GetComponent<BoxCollider>();
            if ( !collider )
            {
                collider = gameObject.AddComponent<BoxCollider> ( );
            }
            Bounds bounds = gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;

            collider.size = new Vector3 ( bounds.size.x * 2.0f, bounds.size.y * 2.0f, 0.2f );
            collider.center = bounds.center;
        }

    }
}
