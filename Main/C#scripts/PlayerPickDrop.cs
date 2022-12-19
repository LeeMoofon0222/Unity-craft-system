using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickDrop : MonoBehaviour
{
    public Transform CamTransform;
    public float reachDistance;
    public Transform item_holder;
    public LayerMask Pick_and_Drop;
    private Grabbable grabbable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {   
            if(grabbable == null)
            {
                if (Physics.Raycast(CamTransform.position, CamTransform.forward, out RaycastHit raycastHit, reachDistance))
                {
                    if (raycastHit.transform.TryGetComponent(out grabbable))
                    {
                        grabbable.Grab(item_holder);
                        //Debug.Log("666");

                    }

                }
            }
            else
            {
                grabbable.Drop();
                grabbable = null;
            }




        }
    }
}
