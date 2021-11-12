using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public Button StartButton;
    public Slider LayerSlider;
    public Dropdown AxisSelector;
    public Text AxisText;
    public GameObject BaseCube;
    private GameObject[,,] Cubes = new GameObject[9,9,9];
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
            foreach (var it in Cubes)
                it.SetActive(true);
            (lastAxis, AxisSelector.value) = (AxisSelector.value, 0);
            return;
        }
        else
        {
            foreach (var it in Cubes)
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
                    Cubes[layer, y, z].SetActive(true);
                break;
            case 2: //along y
                for (int x = 0; x < 9; x++)
                for (int z = 0; z < 9; z++)
                    Cubes[x, layer, z].SetActive(true);
                break;
            case 3: //along z
                for (int x = 0; x < 9; x++)
                for (int y = 0; y < 9; y++)
                    Cubes[x, y, layer].SetActive(true);
                break;
            default:
                throw new Exception("Invalid Axis selection: " + AxisSelector.value);
        }
    }
    
    private void ResetGame()
    {
        foreach (var it in Cubes)
            Destroy(it);
        
        GameObject cube;
        
        for (int z = 0; z < 9; z++)
        for (int x = 0; x < 9; x++)
        for (int y = 0; y < 9; y++)
        {
            var pos = new Vector3(x-4,y-4,z-4);
            cube = Instantiate(BaseCube, pos, Quaternion.identity);
            cube.name = $"Cube(layer={z},[{x},{y}])";
            Cubes[x, y, z] = cube;
        }
    }

    void Update()
    {
    }
}
