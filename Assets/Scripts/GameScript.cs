using System;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public Button ResetViewButton;
    public Button NewGameEasyButton;
    public Button NewGameMediumButton;
    public Button NewGameHardButton;
    public Slider LayerSlider;
    public Dropdown AxisSelector;
    public Text AxisText;
    public Text WinText;
    private int lastAxis = 3;

    private void Start()
    {
        ResetViewButton.onClick.AddListener(ResetView);
        NewGameEasyButton.onClick.AddListener(() => GameState.current.PopulateGame(Difficulty.Easy));
        NewGameMediumButton.onClick.AddListener(() => GameState.current.PopulateGame(Difficulty.Medium));
        NewGameHardButton.onClick.AddListener(() => GameState.current.PopulateGame(Difficulty.Hard));
        LayerSlider.onValueChanged.AddListener(SelectLayer);
        AxisSelector.onValueChanged.AddListener(SelectAxis);
    }

    private void Update()
    {
        // win condition
        foreach (var cell in GameState.current.Cells)
            if (cell == null || cell.Value == 0 || !cell.CheckValid(out _))
                return;
        WinText.gameObject.SetActive(true);
    }

    private void OnApplicationQuit() => GameState.current.Save();

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
        LayerSlider.value = layer;
        layer -= 1;

        if (layer == -1)
        {
            foreach (var it in GameState.current.Cells)
                it.gameObject.SetActive(true);
            (lastAxis, AxisSelector.value) = (AxisSelector.value, 0);
            return;
        }

        foreach (var it in GameState.current.Cells)
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
                    GameState.current.Cells[layer, y, z].gameObject.SetActive(true);
                break;
            case 2: //along y
                for (sbyte x = 0; x < 9; x++)
                for (sbyte z = 0; z < 9; z++)
                    GameState.current.Cells[x, layer, z].gameObject.SetActive(true);
                break;
            case 3: //along z
                for (sbyte x = 0; x < 9; x++)
                for (sbyte y = 0; y < 9; y++)
                    GameState.current.Cells[x, y, layer].gameObject.SetActive(true);
                break;
            default:
                throw new Exception("Invalid Axis selection: " + AxisSelector.value);
        }
    }

    internal void ViewPlanes(Cell cell)
    {
        ViewPlanes(cell.X, cell.Y, cell.Z);
    }

    internal void ViewPlanes(int px, int py, int pz)
    {
        (lastAxis, AxisSelector.value, AxisText.text) = (AxisSelector.value, 0, "Viewing Cell");
        foreach (var it in GameState.current.Cells)
            it.gameObject.SetActive(false);

        for (sbyte y = 0; y < 9; y++)
        for (sbyte z = 0; z < 9; z++)
            GameState.current.Cells[px, y, z].gameObject.SetActive(true);
        for (sbyte x = 0; x < 9; x++)
        for (sbyte z = 0; z < 9; z++)
            GameState.current.Cells[x, py, z].gameObject.SetActive(true);
        for (sbyte x = 0; x < 9; x++)
        for (sbyte y = 0; y < 9; y++)
            GameState.current.Cells[x, y, pz].gameObject.SetActive(true);
    }

    internal void ResetView()
    {
        GameState.current.SelectedCell = null;
        Cell.anySelected = false;
        SelectAxis(0);
    }
}