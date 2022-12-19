using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolderFollow : MonoBehaviour
{

    public Transform player;
    public Transform me;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        me.position = new Vector3(player.position.x, player.position.y, player.position.z);
    }
}
