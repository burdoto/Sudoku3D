using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public Material DefaultMaterial;
    public Material HoverMaterial;
    public Material SelectedMaterial;
    public Material FaultyMaterial;
    public Material CorrectMaterial;

    public Button StartButton;
    public Slider LayerSlider;
    public Dropdown AxisSelector;
    public Text AxisText;
    public GameObject BaseCube;

    internal Cell HoveredCell { get; set; }
    [CanBeNull] 
    internal Cell SelectedCell { get; set; }
    
    private GameObject[,,] Cells = new GameObject[9,9,9];
    private int lastAxis = 3;

    void Start()
    {
        StartButton.onClick.AddListener(ResetGame);
        LayerSlider.onValueChanged.AddListener(SelectLayer);
        AxisSelector.onValueChanged.AddListener(SelectAxis);
        ResetGame();
    }

    private void SelectAxis(int axis)
    {
        lastAxis = axis;
        SelectLayer(LayerSlider.value);
    }

    private void SelectLayer(float layer) => SelectLayer((int)layer);

    private void SelectLayer(int layer)
    {
        AxisText.text = layer == -1 ? "Everything" : layer.ToString();
        layer -= 1;

        if (layer == -1)
        {
            foreach (var it in Cells)
                it.SetActive(true);
            (lastAxis, AxisSelector.value) = (AxisSelector.value, 0);
            return;
        }
        else
        {
            foreach (var it in Cells)
                it.SetActive(false);
            AxisSelector.value = lastAxis == 0 ? 3 : lastAxis;
        }

        switch (AxisSelector.value)
        {
            case 0: //everything
                // is handled above
                break;
            case 1: //along x
                for (int y = 0; y < 9; y++)
                for (int z = 0; z < 9; z++)
                    Cells[layer, y, z].SetActive(true);
                break;
            case 2: //along y
                for (int x = 0; x < 9; x++)
                for (int z = 0; z < 9; z++)
                    Cells[x, layer, z].SetActive(true);
                break;
            case 3: //along z
                for (int x = 0; x < 9; x++)
                for (int y = 0; y < 9; y++)
                    Cells[x, y, layer].SetActive(true);
                break;
            default:
                throw new Exception("Invalid Axis selection: " + AxisSelector.value);
        }
    }
    
    private void ResetGame()
    {
        foreach (var it in Cells)
            Destroy(it);
        
        GameObject cube;
        
        for (int x = 0; x < 9; x++)
        for (int y = 0; y < 9; y++)
        for (int z = 0; z < 9; z++)
        {
            var pos = new Vector3(x-4,y-4,z-4);
            cube = Instantiate(BaseCube, pos, Quaternion.identity);
            cube.name = $"Cube[{x},{y},{z}]";
            Cells[x, y, z] = cube;
        }
    }

    void Update()
    {
        var camera = Camera.main;

        if (camera == null)
            return;
        
        var mouseRay = camera.ScreenPointToRay(Input.mousePosition);
        if (!Input.GetMouseButton(1) 
            && Physics.Raycast(mouseRay, out RaycastHit hit) 
            && hit.collider.gameObject.GetComponent<Cell>() is { } cell
            && cell == HoveredCell 
            && HoveredCell != null
            && Input.GetMouseButtonDown(0))
        {
            Cell.anySelected = true;
            SelectedCell = HoveredCell;
            var renderer = HoveredCell.GetComponent<MeshRenderer>();
            renderer.material = SelectedMaterial;
        }

        if (Cell.anySelected && SelectedCell != null)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                SelectedCell.Value = 0;
                Cell.anySelected = false;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                SelectedCell.Value = 1;
                Cell.anySelected = false;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                SelectedCell.Value = 2;
                Cell.anySelected = false;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                SelectedCell.Value = 3;
                Cell.anySelected = false;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                SelectedCell.Value = 4;
                Cell.anySelected = false;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                SelectedCell.Value = 5;
                Cell.anySelected = false;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                SelectedCell.Value = 6;
                Cell.anySelected = false;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                SelectedCell.Value = 7;
                Cell.anySelected = false;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                SelectedCell.Value = 8;
                Cell.anySelected = false;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                SelectedCell.Value = 9;
                Cell.anySelected = false;
            }
        }
    }
}
