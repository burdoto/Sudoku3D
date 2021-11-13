using System;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public Button StartButton;
    public Button ResetViewButton;
    public Slider LayerSlider;
    public Dropdown AxisSelector;
    public Text AxisText;
    private int lastAxis = 3;

    internal GameState State;

    private void Start()
    {
        State = GetComponent<GameState>();
        StartButton.onClick.AddListener(State.ResetGame);
        ResetViewButton.onClick.AddListener(ResetView);
        LayerSlider.onValueChanged.AddListener(SelectLayer);
        AxisSelector.onValueChanged.AddListener(SelectAxis);
    }

    private void Update()
    {
    }

    internal void SelectAxis(int axis)
    {
        lastAxis = axis;
        SelectLayer(axis == 0 ? 0 : LayerSlider.value);
    }

    internal void SelectLayer(float layer)
    {
        SelectLayer((int)layer);
    }

    internal void SelectLayer(int layer)
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

        foreach (var it in State.Cells)
            it.gameObject.SetActive(false);
        AxisSelector.value = lastAxis == 0 ? 3 : lastAxis;

        switch (AxisSelector.value)
        {
            case 0: //everything
                // is handled above
                break;
            case 1: //along x
                for (sbyte y = 0; y < 9; y++)
                for (sbyte z = 0; z < 9; z++)
                    State.Cells[layer, y, z].gameObject.SetActive(true);
                break;
            case 2: //along y
                for (sbyte x = 0; x < 9; x++)
                for (sbyte z = 0; z < 9; z++)
                    State.Cells[x, layer, z].gameObject.SetActive(true);
                break;
            case 3: //along z
                for (sbyte x = 0; x < 9; x++)
                for (sbyte y = 0; y < 9; y++)
                    State.Cells[x, y, layer].gameObject.SetActive(true);
                break;
            default:
                throw new Exception("Invalid Axis selection: " + AxisSelector.value);
        }
    }

    internal void ViewAxies(Cell cell)
    {
        ViewAxies(cell.X, cell.Y, cell.Z);
    }

    internal void ViewAxies(int px, int py, int pz)
    {
        (lastAxis, AxisSelector.value, AxisText.text) = (AxisSelector.value, 0, "Viewing Cell");
        foreach (var it in State.Cells)
            it.gameObject.SetActive(false);

        for (sbyte y = 0; y < 9; y++)
        for (sbyte z = 0; z < 9; z++)
            State.Cells[px, y, z].gameObject.SetActive(true);
        for (sbyte x = 0; x < 9; x++)
        for (sbyte z = 0; z < 9; z++)
            State.Cells[x, py, z].gameObject.SetActive(true);
        for (sbyte x = 0; x < 9; x++)
        for (sbyte y = 0; y < 9; y++)
            State.Cells[x, y, pz].gameObject.SetActive(true);
    }

    internal void ResetView()
    {
        SelectAxis(lastAxis);
    }
}