using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Clothing.Drawing
{
    public class PhotoHelper
    {
        int width;
        int height;
        Color[] caches;
        private Texture2D _photo;
        public Texture2D Photo
        {
            get
            {
                if(_photo == null )
                {
                    _photo = Resources.Load<Texture2D> ( "Girl" );// ( "Cat" );
                    int miplevel = 1;
                    int powOfTwo = 1 << miplevel;

                    width = _photo.width / powOfTwo;
                    height = _photo.height / powOfTwo;
                    caches = _photo.GetPixels ( miplevel );
                        
                }
                return _photo;
            }
            set
            {
                _photo = value;
            }
        }

        public bool RaycastHitToUVCoordinate ( Vector3 position, out Vector2 uv )
        {
            uv = Vector2.zero;
            var origin = new Vector3(position.x, position.y, -10);
            var layers = 1 << LayerMask.NameToLayer("Canvas");
            RaycastHit hit;
            if ( Physics.Raycast ( origin, Vector3.forward, out hit, Mathf.Infinity, layers ) )
            {
                uv = hit.textureCoord;
                return true;
            }
            return false;
        }
         
        public Vector2 ConvertPositionToUV(Vector3 position )
        {
            var origin = new Vector3(position.x, position.y, -10);
            var layers = 1 << LayerMask.NameToLayer("Canvas");
            RaycastHit hit;
            if ( Physics.Raycast ( origin, Vector3.forward, out hit, Mathf.Infinity, layers ) )
            {
                return hit.textureCoord;
                ///return true;
            }
            return Vector2.zero;
        }
    }
}
