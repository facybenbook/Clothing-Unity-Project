using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class _GLUtils : MonoBehaviour
{

    #region GUI
    
    private static Material lineMaterial;
    
    //public List<Vector3> Points{ set; get; }
    public List<List<Vector3>> Paths { set; get; }
    
    public Color LineColor = new Color(0.918F, 0.422F, 0.012F);// Color.red;
    public bool Enable = false;

    public void Reset ( )
    {
        Enable = true;
        Paths = new List<List<Vector3>> ( );
        //Points = null;
    }

    private static void CreateLineMaterial ( )
    {
        if ( !lineMaterial )
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material ( shader );
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt ( "_SrcBlend", ( int ) UnityEngine.Rendering.BlendMode.SrcAlpha );
            lineMaterial.SetInt ( "_DstBlend", ( int ) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha );
            // Turn backface culling off
            lineMaterial.SetInt ( "_Cull", ( int ) UnityEngine.Rendering.CullMode.Off );
            // Turn off depth writes
            lineMaterial.SetInt ( "_ZWrite", 0 );
        }
    }

    //public Rect position = new Rect (16, 16, 128, 24);

    private void OnGUI ( )
    {
        if ( Enable)// && Points != null && Points.Count > 0 )
        {
            Draw ( );
        }
    }

    private void Draw ( )
    {
        CreateLineMaterial ( );

        lineMaterial.SetPass ( 0 );

        GL.PushMatrix ( );


        GL.LoadPixelMatrix ( );

        GL.Begin ( GL.LINES );

        GL.Color ( LineColor );
        
        foreach(var path in Paths )
        {
            if ( path != null )
            {
                var previces = path.LastOrDefault();
                if ( previces != null )
                {
                    foreach ( var current in path )
                    {
                        //OnDrawDotLine ( previces, current );
                        GL.Vertex ( previces );
                        GL.Vertex ( current );
                        previces = current;
                    }
                }
            }
        }

        
        
        GL.End ( );
        GL.PopMatrix ( );
    }
    
    //List<Vector3> WorldToScreen(Rect bounds )
    //{
    //    var context = UISystem.FindWindow<BoxContext>();
    //    //var cam = context.Camera2d;
    //    var min =  ProjectCam.WorldToScreenPoint( transform.TransformPoint(bounds.min));
    //    var max =  ProjectCam.WorldToScreenPoint( transform.TransformPoint(bounds.max));
    //    return new List<Vector3> ( )
    //    {
    //        new Vector3(min.x, min.y, 1),
    //        new Vector3(min.x, max.y, 1),
    //        new Vector3(max.x, max.y,1),
    //        new Vector3(max.x, min.y, 1)
    //    };
    //}
    
    private void OnDrawDotLine ( Vector3 from, Vector3 to, float dash = 10F, float gap = 5F, float phase = 5F )
    {
        var distance = Vector3.Distance(from, to);
        var size = (int)(distance / dash);

        var actul_dash = distance / size;
        var actul_phase = (actul_dash - gap) * 0.5F;

        for ( var i = 0; i < size + 1; i++ )
        {
            var start = (i * actul_dash - actul_phase) / distance;
            var end = ((i + 1) * actul_dash - gap - actul_phase) / distance;
            if ( start < 0 ) start = 0;
            if ( end > 1 ) end = 1;
            var startPoint = Vector3.Lerp(from, to, start);
            var endPoint = Vector3.Lerp(from, to, end);
            GL.Vertex ( startPoint );
            GL.Vertex ( endPoint );
        }
    }

    #endregion GUI
}
