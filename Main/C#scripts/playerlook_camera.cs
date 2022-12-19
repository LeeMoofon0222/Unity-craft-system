using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerlook_camera : MonoBehaviour
{
    public float MouseSensitivity = 100f;

    public GameObject PlayerBody;

    public Move move;
    

    float xRotation = 0f;
    float yRotation = 0f;
    

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        
    }

    // Update is called once per frame
    void Update()
    {
        float mX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime; //GETAXIS是案件觸發動作用的程式碼
        float mY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime * 0.5f;

        xRotation -= mY;
        yRotation += mX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);


        //Debug.Log(mX);
        PlayerBody.transform.rotation = Quaternion.Lerp(PlayerBody.transform.localRotation, Quaternion.Euler(0f, yRotation, move.wr_zRotation), Time.deltaTime * 100f); ;

        Cursor.visible = false;
    }
}
