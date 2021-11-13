using JetBrains.Annotations;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState current;

    public Material DefaultMaterial;
    public Material HoverMaterial;
    public Material SelectedMaterial;
    public Material FaultyMaterial;
    public Material CorrectMaterial;
    public GameObject BaseCube;

    internal Cell[,,] Cells = new Cell[9, 9, 9];

    internal GameScript Game;
    internal Cell HoveredCell { get; set; }

    [CanBeNull] internal Cell SelectedCell { get; set; }

    private void Awake()
    {
        Game = GetComponent<GameScript>();
        current = this;
    }

    private void Start()
    {
        ResetGame();
        PopulateGame();
    }

    private void Update()
    {
        // selecting hovered cell
        if (!Input.GetMouseButton(1)
            && HoveredCell != null
            && Input.GetMouseButtonDown(0))
        {
            Cell.anySelected = true;
            SelectedCell = HoveredCell;
            var renderer = HoveredCell.GetComponent<MeshRenderer>();
            renderer.material = SelectedMaterial;
            Game.ViewAxies(HoveredCell);
        }

        // input
        if (Cell.anySelected && SelectedCell != null)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                SelectedCell.Value = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                SelectedCell.Value = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                SelectedCell.Value = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                SelectedCell.Value = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                SelectedCell.Value = 4;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                SelectedCell.Value = 5;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                SelectedCell.Value = 6;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                SelectedCell.Value = 7;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                SelectedCell.Value = 8;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                SelectedCell.Value = 9;
            }
            else if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SelectedCell.TryLock();
                SelectedCell = null;
                Cell.anySelected = false;
            }
        }
    }

    internal void ResetGame()
    {
        foreach (var it in Cells)
            if (it != null && it.gameObject is { } go)
                Destroy(go);

        for (sbyte x = 0; x < 9; x++)
        for (sbyte y = 0; y < 9; y++)
        for (sbyte z = 0; z < 9; z++)
        {
            var pos = new Vector3(x - 4, y - 4, z - 4);
            var cube = Instantiate(BaseCube, pos, Quaternion.identity);
            var cell = Cells[x, y, z] = cube.GetComponent<Cell>();
            cell.X = x;
            cell.Y = y;
            cell.Z = z;
            cube.name = cell.ToString();
        }
    }

    internal void PopulateGame()
    {
        sbyte[,,] yield = new sbyte[9, 9, 9];
        
        // todo
        
        for (sbyte x = 0; x < 9; x++)
        for (sbyte y = 0; y < 9; y++)
        for (sbyte z = 0; z < 9; z++)
            Cells[x, y, z].Value = yield[x, y, z];
    }
}