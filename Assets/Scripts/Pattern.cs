using UnityEngine;

[CreateAssetMenu(menuName = "Pattern", fileName = "New Pattern")]
public class Pattern : ScriptableObject
{
    public Vector2Int[] cells;

    public bool IsEmpty => cells == null || cells.Length == 0;

    public Vector2Int GetMin()
    {
        if (IsEmpty) return Vector2Int.zero;
        Vector2Int min = cells[0];
        for (int i = 1; i < cells.Length; i++)
        {
            min.x = Mathf.Min(min.x, cells[i].x);
            min.y = Mathf.Min(min.y, cells[i].y);
        }
        return min;
    }

    public Vector2Int GetMax()
    {
        if (IsEmpty) return Vector2Int.zero;
        Vector2Int max = cells[0];
        for (int i = 1; i < cells.Length; i++)
        {
            max.x = Mathf.Max(max.x, cells[i].x);
            max.y = Mathf.Max(max.y, cells[i].y);
        }
        return max;
    }

    /// <summary>
    /// Center (integer-truncated) of bounding box: (min + max) / 2
    /// </summary>
    public Vector2Int GetCenter()
    {
        if (IsEmpty) return Vector2Int.zero;
        return (GetMin() + GetMax()) / 2;
    }

    public int GetWidth()   // inclusive number of cells horizontally
    {
        if (IsEmpty) return 0;
        Vector2Int min = GetMin();
        Vector2Int max = GetMax();
        return Mathf.Abs(max.x - min.x) + 1;
    }

    public int GetHeight()  // inclusive number of cells vertically
    {
        if (IsEmpty) return 0;
        Vector2Int min = GetMin();
        Vector2Int max = GetMax();
        return Mathf.Abs(max.y - min.y) + 1;
    }

    public int GetLargestSide()
    {
        return Mathf.Max(GetWidth(), GetHeight());
    }

    /// <summary>
    /// Returns all cells normalized such that the pattern's bounding-box center becomes (0,0).
    /// Use this for placing the pattern centered around the origin.
    /// </summary>
    public Vector3Int[] GetCellsCentered()
    {
        if (IsEmpty) return new Vector3Int[0];
        Vector2Int center = GetCenter();
        Vector3Int[] result = new Vector3Int[cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            Vector2Int p = cells[i] - center;
            result[i] = new Vector3Int(p.x, p.y, 0);
        }
        return result;
    }
}
