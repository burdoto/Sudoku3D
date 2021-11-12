using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public TextMesh NumTextMesh => GetComponentInChildren<TextMesh>();

    private sbyte num = 0;
    public sbyte Value
    {
        get => num;
        set => NumTextMesh.text = (num = value).ToString();
    }

    void Update()
    {
        var transform = NumTextMesh.transform;
        var camera = Camera.main;

        var game = camera!.GetComponent<GameScript>();
        if (game.LayerSlider.value == 0)
            NumTextMesh.text = "";
        else
        {
            NumTextMesh.text = num.ToString();
            transform.LookAt(camera!.transform);
            transform.rotation *= Quaternion.Euler(0,180,0);
        }
    }
}
