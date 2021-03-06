using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Difficulty
{
    Hard = 2,
    Medium = 4,
    Easy = 5
}

public static class DebugCheck
{
    public const bool PrintPopulate = false;
    public const bool PrintConflict = false;
}

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
    [CanBeNull]
    internal Cell HoveredCell { get; set; }
    [CanBeNull] 
    internal Cell SelectedCell { get; set; }

    private void Awake()
    {
        Debug.unityLogger.Log("Savegame path: " + GetSavegamePath());
        Game = GetComponent<GameScript>();
        current = this;
    }

    private void Start()
    {
        if (File.Exists(GetSavegamePath()))
        {
            CreateCubes();
            Load();
        }
        else
        {
            ResetGame();
            CreateCubes();
            PopulateGame();
        }
    }

    private void Update()
    {
        // selecting hovered cell on click
        if (!GInput.IsHold
            && GInput.IsTap
            && HoveredCell != null
            && HoveredCell.IsHovered())
        {
            Cell.anySelected = true;
            SelectedCell = HoveredCell;
            var renderer = HoveredCell.GetComponent<MeshRenderer>();
            renderer.material = SelectedMaterial;
            if (Game.LayerSlider.value == 0)
                Game.ViewPlanes(HoveredCell);
        }

        // input
        if (Cell.anySelected && SelectedCell != null)
        {
            if (!GInput.IsMobile)
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
            else Numpad.Read(byt =>
            {
                if (byt == -1)
                {
                    SelectedCell.TryLock();
                    SelectedCell = null;
                    Cell.anySelected = false;
                }
                else SelectedCell.Value = byt;
            });
        }
    }

    internal void ResetGame()
    {
        File.Delete(GetSavegamePath());
        
        Game.WinText.gameObject.SetActive(false);
        
        foreach (var it in Cells)
            if (it != null && it.gameObject is { } go)
                Destroy(go);
    }

    private void CreateCubes()
    {
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

    internal void PopulateGame() => PopulateGame(Difficulty.Medium);

    internal void PopulateGame(Difficulty difficulty)
    {
#pragma warning disable 162
        if (DebugCheck.PrintPopulate)
            Debug.unityLogger.Log("PopulateGame started");
        
        
        if (DebugCheck.PrintPopulate)
            Debug.unityLogger.Log("PopulateGame calling ResetGame");
        ResetGame();
        if (DebugCheck.PrintPopulate)
            Debug.unityLogger.Log("PopulateGame calling CreateCubes");
        CreateCubes();
        
        // obtained via: 17 * root(3, 17)
        // 17 is minimum hints for 2d sudoku
        const int minFields = 44; // min visible fields
        int limit = minFields * (int) difficulty;
        
        if (DebugCheck.PrintPopulate)
            Debug.unityLogger.Log("PopulateGame entering loop 1");
        for (sbyte i = 0; i < 9; i++)
            Cells[i, i, i].Value = (sbyte)(i+1);
        if (DebugCheck.PrintPopulate)
            Debug.unityLogger.Log("PopulateGame entering loop 2");
        for (int i = 0; i < limit; i++)
        {
            int c0 = 100, c1;
            bool success;
            Cell cell;
            if (DebugCheck.PrintPopulate)
                Debug.unityLogger.Log("PopulateGame entering loop 3");
            do
            {
                c1 = 9;
                cell = Cells[Random.Range(0, 9), Random.Range(0, 9), Random.Range(0, 9)];
                //Debug.unityLogger.Log("PopulateGame entering loop 4");
                do cell.Value = (sbyte)Random.Range(1, 10);
                while (--c1 > -1 && !cell.CheckValid(out _));
                success = c1 > -1;
            } while (!success && --c0 > 0);
            
            if (DebugCheck.PrintPopulate)
                Debug.unityLogger.Log("PopulateGame Debug", $"Counters: 0={c0} ; 1={c1}");

            cell.TryLock(CellState.Predefined);
        }
        
        if (DebugCheck.PrintPopulate)
            Debug.unityLogger.Log("PopulateGame finished");
#pragma warning restore 162
    }

    internal void Load()
    {
        using (var stream = new StreamReader(GetSavegamePath(), Encoding.ASCII))
            for (sbyte x = 0; x < 9; x++)
            for (sbyte y = 0; y < 9; y++)
            {
                for (sbyte z = 0; z < 9; z++)
                {
                    var cell = Cells[x, y, z];
                    cell.Value = (sbyte)int.Parse(((char)stream.Read()).ToString());
                    cell.TryLock(CellState.Predefined);
                }

                if (stream.Read() != '\n')
                {
                    PopulateGame();
                    return;
                }
            }
    }

    internal void Save()
    {
        using (var stream = new StreamWriter(GetSavegamePath(), false, Encoding.ASCII))
            for (sbyte x = 0; x < 9; x++)
            for (sbyte y = 0; y < 9; y++)
            {
                for (sbyte z = 0; z < 9; z++)
                    stream.Write(Cells[x, y, z].Value);

                stream.Write('\n');
            }
    }

    public static string GetSavegamePath() => Path.Combine(Application.persistentDataPath, "Sudoku3D.sav");
}