using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public TextMesh NumTextMesh => GetComponentInChildren<TextMesh>();

    internal static bool anySelected;
    private sbyte num;

    public sbyte Value
    {
        get => num;
        set => NumTextMesh.text = (num = value).ToString();
    }

    void Update()
    {
        var transform = NumTextMesh.transform;
        var camera = Camera.main;

        if (camera == null)
            return;

        var game = camera.GetComponent<GameScript>();
        if (num == 0)
            NumTextMesh.text = "";
        else
        {
            NumTextMesh.text = num.ToString();
            transform.LookAt(camera.transform);
            transform.rotation *= Quaternion.Euler(0,180,0);
        }

        if (!anySelected)
        {
            var mouseRay = camera.ScreenPointToRay(Input.mousePosition);
            var renderer = GetComponent<MeshRenderer>();
            if (!Input.GetMouseButton(1) 
                && Physics.Raycast(mouseRay, out RaycastHit hit) 
                && hit.collider.gameObject.GetComponent<Cell>() is { } cell
                && cell == this)
            {
                renderer.material = game.HoverMaterial;
                game.HoveredCell = this;
            } else renderer.material = game.DefaultMaterial;
        }
    }
}
