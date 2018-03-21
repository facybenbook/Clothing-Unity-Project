using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Sew : MonoBehaviour
{

    public JointDemo first;
    public JointDemo second;

    public void Execute ( )
    {
        var firstUp = first.rigid_Up.OrderBy(o=>o.transform.position.x);
        var secondUp = second.rigid_Up.OrderBy(o=>o.transform.position.x);
        ConnectBody ( firstUp, secondUp );

        var firstDown = first.rigid_Down.OrderBy(o=>o.transform.position.x);
        var secondDown = second.rigid_Down.OrderBy(o=>o.transform.position.x);
        ConnectBody ( firstDown, secondDown );

        var firstLeft = first.rigid_Left.OrderBy(o=>o.transform.position.y);
        var secondLeft = second.rigid_Left.OrderBy(o=>o.transform.position.y);


        var firstRight = first.rigid_Right.OrderBy(o=>o.transform.position.y);
        var secondRight = second.rigid_Right.OrderBy(o=>o.transform.position.y);

        ConnectBody ( firstLeft, secondRight );

        ConnectBody ( firstRight, secondLeft );

        IsSewn = true;
        //StartCoroutine ( OnSewing ( ) );

    }

    bool IsSewn = false;

    int count = 0;
    int size = 64;
    void Update ( )
    {
        if (IsSewn && count < size )
        {

            var firstPoints = first.GetComponentsInChildren<Rigidbody> ( ).Select(r=>r.position);
            var secondPoints = second.GetComponentsInChildren<Rigidbody>().Select(r=>r.position);
            var bounds = firstPoints.Union(secondPoints).Bounds();
            var center = bounds.center;

            foreach ( var rigid in first.others.Union(second.others ))
            {
                //rigid.useGravity = true;
                var vector = (rigid.position - center);
                var dir = vector.normalized;
                var dis = vector.magnitude;
                rigid.AddForce ( dir / dis);
                //rigid.useGravity = true;

            }
            
            count++;
        }
    }


    void ConnectBody ( IOrderedEnumerable<Rigidbody> first, IOrderedEnumerable<Rigidbody> second )
    {
        for ( int i = 0; i < first.Count ( ); i++ )
        {
            var rigid_0 = first.ElementAt ( i );
            var rigid_1 = second.ElementAt( i );
            var spring = rigid_0.gameObject.AddComponent<SpringJoint>();
            spring.connectedBody = rigid_1;
            spring.minDistance = 0F;
            spring.maxDistance = 0F;
            spring.spring = 1F;
            spring.damper = 0F;
            spring.autoConfigureConnectedAnchor = false;
            spring.enableCollision = false;
            spring.connectedAnchor = spring.anchor = Vector3.zero;
            spring.axis = Vector3.back;
            spring.tolerance = 0.01F;
            spring.enablePreprocessing = false;
        }
    }

    void OnGUI ( )
    {
        if ( GUI.Button ( new Rect ( 0, 0, 80, 32 ), "Sew" ) )
        {
            //var mesh = new Mesh();
            //GetComponent<SkinnedMeshRenderer> ( ).BakeMesh ( mesh );
            //backMesh.mesh = mesh;

            Execute ( );

        }
    }

}
