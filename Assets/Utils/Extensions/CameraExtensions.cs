using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Extensions
{
    public class RaycastArgs
    {
        public Vector3 WorldPosition { set; get; }
        public Vector2 TextureCoord { set; get; }
    }

    public static class CameraExtensions
    {
        public static bool ScreenPointToWorldPointInRectangle ( this Camera cam, Vector3 planePosition, Quaternion planeRotation, Vector2 screenPoint, out Vector3 worldPoint )
        {
            worldPoint = Vector2.zero;
            Ray ray = RectTransformUtility.ScreenPointToRay(cam, screenPoint);
            Plane plane = new Plane( planeRotation * Vector3.back, planePosition);
            float distance;
            bool result;
            if ( !plane.Raycast ( ray, out distance ) )
            {
                result = false;
            }
            else
            {
                worldPoint = ray.GetPoint ( distance );
                result = true;
            }
            return result;
        }

        public static bool ScreenPointRaycastResult(this Camera cam, Vector2 screenPoint, out RaycastArgs args )
        {
            args = null;
            Ray ray = cam.ScreenPointToRay(screenPoint);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity ) )
            {
                args.TextureCoord = hit.textureCoord;
                args.WorldPosition = ray.GetPoint(hit.distance);
                return true;
            }
            return false;
        }

    }
}

