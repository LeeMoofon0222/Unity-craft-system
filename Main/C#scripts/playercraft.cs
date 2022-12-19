using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercraft : MonoBehaviour
{
    [SerializeField] private Transform playercamaratransform;
    [SerializeField] private LayerMask interactlayermask;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float interactdis = 3f;
            if(Physics.Raycast(playercamaratransform.position, playercamaratransform.forward, out RaycastHit raycastHit, interactdis))
            {
                if(raycastHit.transform.TryGetComponent(out craftanvil craftanvil))
                {
                    craftanvil.Craft();
                }
             
            }
        }
    }
}
