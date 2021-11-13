using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public TextMesh NumTextMesh => GetComponentInChildren<TextMesh>();
    
    internal static bool anySelected;
    internal sbyte X,Y,Z;
    internal bool locked = false, faulty = false;
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

        if (true)//!anySelected)
        {
            var mouseRay = camera.ScreenPointToRay(Input.mousePosition);
            var renderer = GetComponent<MeshRenderer>();

            if (faulty)
            {
                renderer.material = GameState.current.FaultyMaterial;
            }
            else if (locked)
            {
                renderer.material = GameState.current.CorrectMaterial;
            }
            else if (game.State.SelectedCell == this || !Input.GetMouseButton(1) && IsHovered())
            {
                renderer.material = GameState.current.HoverMaterial;
                GameState.current.HoveredCell = this;
            } else renderer.material = GameState.current.DefaultMaterial;
        }
    }

    internal bool IsHovered()
    {
        var camera = Camera.main;

        if (camera == null)
            return false;
        
        // selecting
        var mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(mouseRay, out RaycastHit hit)
            && hit.collider.gameObject.GetComponent<Cell>() is { } cell
            && cell == this;
    }
}
