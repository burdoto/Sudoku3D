using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float Multiplier = 2.5f;

    private void Start()
    {
        transform.LookAt(Vector3.zero);
    }

    private void Update()
    {
        var transform = this.transform;
        transform.position += transform.forward * GInput.ZoomAxis * Multiplier;

        if (!GInput.IsHold)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.RotateAround(Vector3.zero, Vector3.up, GInput.CursorDelta.x * Multiplier);
        transform.RotateAround(Vector3.zero, transform.right, -GInput.CursorDelta.y * Multiplier);
    }
}