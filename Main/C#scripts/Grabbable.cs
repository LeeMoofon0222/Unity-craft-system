using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{

    private Rigidbody object_rb;
    private Transform objectGrabPointTransform;


    private void Awake()
    {   
        object_rb = GetComponent<Rigidbody>(); 
        
    }


    public void Grab(Transform item_holder)
    {
        this.objectGrabPointTransform = item_holder;
        object_rb.useGravity = false;
        //object_rb.isKinematic = true;
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        object_rb.useGravity = true;
    }

    private void FixedUpdate()
    {
        if(objectGrabPointTransform != null)
        {
            object_rb.MovePosition(objectGrabPointTransform.position);


        }
        
    }
}
