using UnityEngine;
using System.Collections;

namespace ZERO.Extensions
{
    public static class _TextureExtensions
    {

        public static void WriteTo ( this Texture2D srcTex, out Texture2D destTex )//, Material material = null)
        {
            var width = srcTex.width;
            var height = srcTex.height;
            
            int dimension = Mathf.Max(width, height);

            if ( !Mathf.IsPowerOfTwo ( width ) || !Mathf.IsPowerOfTwo ( height ) || width != height )
            {
                var wPOT = Mathf.NextPowerOfTwo(width);
                var hPOT = Mathf.NextPowerOfTwo(height);

                dimension = Mathf.Max ( wPOT, hPOT );

            }

            // Create a temporary RenderTexture of the same size as the texture
            RenderTexture tmp = RenderTexture.GetTemporary(
                           dimension,//width,
                           dimension,//height,
                            0,
                            RenderTextureFormat.ARGB32,
                            RenderTextureReadWrite.Linear);


            // Create a new readable Texture2D to copy the pixels to it
            destTex = new Texture2D ( dimension, dimension, TextureFormat.ARGB32, false, true );// , format);//, texture.format, false);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit ( srcTex, tmp );//, material);
                                          // Backup the currently set RenderTexture
                                          //RenderTexture previous = RenderTexture.active;
                                          // Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;

            //DrawGLVertices(width, height, material);

            // Copy the pixels from the RenderTexture to the new Texture
            destTex.ReadPixels ( new Rect ( 0, 0, dimension, dimension ), 0, 0 );
            destTex.Apply ( );
            // Reset the active RenderTexture
            RenderTexture.active = null;// previous;
                                        // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary ( tmp );
        }
        
        public static void Extend(this Texture2D srcTex )
        {
            var srcW = srcTex.width;
            var height = srcTex.height;


        }

    }
}
