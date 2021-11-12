using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float Multiplier = 2.5f;
    
    void Update()
    {
        transform.position += transform.forward * Input.GetAxis("Mouse ScrollWheel") * Multiplier;
        
        if (!Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.RotateAround(Vector3.zero, Vector3.up, Input.GetAxis("Mouse X") * Multiplier);
        //transform.rotation.eulerAngles.x += Input.GetAxis("Mouse Y");
    }
}
