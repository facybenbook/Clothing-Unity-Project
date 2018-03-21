using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragHandlerTest : MonoBehaviour {

    private Vector3 rawPosition;
    private Vector3 deltaPosition;


    public void HandleOnBeginDrag ( Vector3 worldPosition )
    {
        rawPosition = transform.position;
        deltaPosition = transform.position - worldPosition;

    }

    public void HandleOnDrag(Vector3 worldPosition )
    {
        transform.position = worldPosition + deltaPosition;
    }
	

    public void HandleOnDragOut ( )
    {
        transform.position = rawPosition;
    }

    public void HandleOnGotFocus ( )
    {
        GetComponent<Renderer> ( ).material.color = Color.red;
    }

    public void HandleOnLostFocus ( )
    {
        GetComponent<Renderer> ( ).material.color = Color.white;
    }
}
