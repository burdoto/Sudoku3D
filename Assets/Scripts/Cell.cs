using JetBrains.Annotations;
using UnityEngine;

public enum CellState
{
    Normal,
    Faulty,
    Locked,
    Predefined
}

public class Cell : MonoBehaviour
{
    internal static bool anySelected;

    [CanBeNull] internal Cell conflicting;

    private sbyte num;
    internal CellState State;
    internal sbyte X, Y, Z;
    public TextMesh NumTextMesh => GetComponentInChildren<TextMesh>();

    public sbyte Value
    {
        get => num;
        set => NumTextMesh.text = (num = value).ToString();
    }

    private void Update()
    {
        var transform = NumTextMesh.transform;
        var camera = Camera.main;

        if (camera == null)
            return;

        var game = camera.GetComponent<GameScript>();
        if (num == 0)
        {
            NumTextMesh.text = "";
        }
        else
        {
            NumTextMesh.text = num.ToString();
            transform.LookAt(camera.transform);
            transform.rotation *= Quaternion.Euler(0, 180, 0);
        }

        if (true) //!anySelected)
        {
            var renderer = GetComponent<MeshRenderer>();

            if (State == CellState.Faulty && !IsHovered())
            {
                renderer.material = GameState.current.FaultyMaterial;
            }
            else if (State == CellState.Predefined || State == CellState.Locked)
            {
                renderer.material = GameState.current.CorrectMaterial;
            }
            else if (!GInput.IsHold && IsHovered())
            {
                renderer.material = GameState.current.HoverMaterial;
                GameState.current.HoveredCell = this;
            }
            else if (GameState.current.SelectedCell == this)
            {
                renderer.material = GameState.current.SelectedMaterial;
            }
            else
            {
                renderer.material = GameState.current.DefaultMaterial;
            }
        }
    }

    internal bool IsHovered()
    {
        var camera = Camera.main;

        if (camera == null)
            return false;

        // selecting
        var mouseRay = camera.ScreenPointToRay(GInput.CursorPosition);
        return Physics.Raycast(mouseRay, out var hit)
               && hit.collider.gameObject.GetComponent<Cell>() is { } cell
               && cell == this;
    }

    // checks all axies cells for validity, then locks if possible
    internal void TryLock(CellState inState = CellState.Locked)
    {
        if (num == 0)
            return;
        Cell cell;
        State = CellState.Normal;

        if (!CheckValid(out cell) && cell != null)
        {
            //Debug.unityLogger.Log($"Conflict detected: cell {this} and {cell}");
            cell.State = State = CellState.Faulty;
            (cell.conflicting, conflicting) = (this, cell);
        }

        if (State != CellState.Faulty)
        {
            if (conflicting != null && conflicting.CheckValid(out _))
                (conflicting.State, conflicting) = (CellState.Normal, null);
            State = inState;
        }
    }

    // checks whether ONLY THIS cell is valid
    internal bool CheckValid([CanBeNull] out Cell conflict)
    {
#pragma warning disable 162
        conflict = null;
        if (num == 0)
            return true;

        // check lines
        for (sbyte x = 0; x < 9; x++)
        for (sbyte y = 0; y < 9; y++)
        for (sbyte z = 0; z < 9; z++)
            if (num == (conflict = GameState.current.Cells[x, Y, Z]).num && conflict != this)
                return false;
            else if (num == (conflict = GameState.current.Cells[X, y, Z]).num && conflict != this)
                return false;
            else if (num == (conflict = GameState.current.Cells[X, Y, z]).num && conflict != this) return false;

        // check blocks
        var sqx = X / 3;
        var sqy = Y / 3;
        var sqz = Z / 3;

        int sqxa = 3 * sqx, sqxb = 3 * (sqx + 1) - 1;
        int sqya = 3 * sqy, sqyb = 3 * (sqy + 1) - 1;
        int sqza = 3 * sqz, sqzb = 3 * (sqz + 1) - 1;

        if (DebugCheck.PrintConflict)
        {
            Debug.unityLogger.Log($"X={X} ; Y={Y} ; Z={Z}");
            Debug.unityLogger.Log($"sqx={sqx} ; sqxa={sqxa} ; sqxb={sqxb}");
            Debug.unityLogger.Log($"sqy={sqy} ; sqya={sqya} ; sqyb={sqyb}");
            Debug.unityLogger.Log($"sqz={sqz} ; sqza={sqza} ; sqzb={sqzb}");
        }

        for (var x = sqxa; x <= sqxb; x++)
        for (var y = sqya; y <= sqyb; y++)
        {
            conflict = GameState.current.Cells[x, y, Z];
            if (conflict != this && conflict.num != 0 && num == conflict.num)
            {
                if (DebugCheck.PrintConflict)
                    Debug.unityLogger.Log($"Conflict detected when checking x={x};y={y}: cell {this} and {conflict}");
                return false;
            }
        }

        for (var x = sqxa; x <= sqxb; x++)
        for (var z = sqza; z <= sqzb; z++)
        {
            conflict = GameState.current.Cells[x, Y, z];
            if (conflict != this && conflict.num != 0 && num == conflict.num)
            {
                if (DebugCheck.PrintConflict)
                    Debug.unityLogger.Log($"Conflict detected when checking x={x};z={z}: cell {this} and {conflict}");
                return false;
            }
        }

        for (var y = sqya; y <= sqyb; y++)
        for (var z = sqza; z <= sqzb; z++)
        {
            conflict = GameState.current.Cells[X, y, z];
            if (conflict != this && conflict.num != 0 && num == conflict.num)
            {
                if (DebugCheck.PrintConflict)
                    Debug.unityLogger.Log($"Conflict detected when checking y={y};z={z}: cell {this} and {conflict}");
                return false;
            }
        }

        return true;
#pragma warning restore 162
    }

    public override string ToString()
    {
        return $"Cell[{X},{Y},{Z},value={num}]";
    }
}