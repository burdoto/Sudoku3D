using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public Button StartButton;
    public Slider LayerSlider;
    public Dropdown AxisSelector;
    public Text AxisText;

    internal GameState State; 
    private int lastAxis = 3;

    void Start()
    {
        State = GetComponent<GameState>();
        StartButton.onClick.AddListener(State.ResetGame);
        LayerSlider.onValueChanged.AddListener(SelectLayer);
        AxisSelector.onValueChanged.AddListener(SelectAxis);
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
            foreach (var it in State.Cells)
                it.gameObject.SetActive(true);
            (lastAxis, AxisSelector.value) = (AxisSelector.value, 0);
            return;
        }
        else
        {
            foreach (var it in State.Cells)
                it.gameObject.SetActive(false);
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
                    State.Cells[layer, y, z].gameObject.SetActive(true);
                break;
            case 2: //along y
                for (int x = 0; x < 9; x++)
                for (int z = 0; z < 9; z++)
                    State.Cells[x, layer, z].gameObject.SetActive(true);
                break;
            case 3: //along z
                for (int x = 0; x < 9; x++)
                for (int y = 0; y < 9; y++)
                    State.Cells[x, y, layer].gameObject.SetActive(true);
                break;
            default:
                throw new Exception("Invalid Axis selection: " + AxisSelector.value);
        }
    }

    void Update()
    {
    }
}
