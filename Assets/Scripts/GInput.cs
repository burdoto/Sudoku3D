using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GInput : MonoBehaviour
{
    public static Vector2 CursorDelta { get; private set; }
    public static Vector2 CursorPosition { get; private set; }
    public static bool IsTap { get; private set; }
    public static bool IsHold { get; private set; }
    public static float ZoomAxis { get; private set; }

    void Update()
    {
        var touches = Input.touches;
        
        if (touches.Length > 0)
            CursorPosition = touches[0].position;

        if (touches.Length == 2)
        {
            // zoom
            ZoomAxis = Vector2.Distance(touches[0].position, touches[1].position);
        }
        else if (touches.Length == 1)
        {
            if (touches[0].tapCount == 1)
            {
                // click
                IsTap = true;
                IsHold = touches[0].phase != TouchPhase.Ended && touches[0].phase != TouchPhase.Canceled;
            }
            // move
            CursorDelta = touches[0].deltaPosition * 0.75f;
        } else if (touches.Length == 0)
        {
            CursorDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            CursorPosition = Input.mousePosition;
            IsTap = Input.GetMouseButtonDown(0);
            IsHold = Input.GetMouseButton(1);
            ZoomAxis = Input.GetAxis("Mouse ScrollWheel");
        }
    }
}
