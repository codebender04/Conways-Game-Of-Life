using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    public static GameBoard Instance;
    [Header("Tilemap & Tiles")]
    [SerializeField] private Tilemap currentState; // assign an empty Tilemap
    [SerializeField] private Tile aliveTile;
    // (optional) deadTile is not needed; we'll clear empty cells by setting null

    [Header("Pattern & Timing")]
    [SerializeField] private float updateInterval = 0.05f;

    [Header("Camera Fit")]
    [Tooltip("Extra padding (in tiles) to leave around the pattern when fitting the camera")]
    [SerializeField] private float cameraPadding = 2f;
    [SerializeField] private Camera targetCamera; // optional; if null use Camera.main

    private Pattern pattern;
    // public stats
    public int Populations { get; private set; }
    public int Iterations { get; private set; }
    public float TimeElapsed { get; private set; }

    // internal state
    private HashSet<Vector3Int> aliveCells;
    private HashSet<Vector3Int> cellsToCheck;
    private Coroutine simulateCoroutine;
    private void Awake()
    {
        Instance = this;
        aliveCells = new HashSet<Vector3Int>();
        cellsToCheck = new HashSet<Vector3Int>();
        if (targetCamera == null) targetCamera = Camera.main;
    }

    private void Start()
    {
        if (pattern != null)
        {
            SetPattern(pattern);
        }
        else
        {
            ClearBoard();
        }

    }

    #region Pattern / Board setup
    private void ClearBoard()
    {
        currentState.ClearAllTiles();
        aliveCells.Clear();
        cellsToCheck.Clear();
        Populations = 0;
        Iterations = 0;
        TimeElapsed = 0f;
    }
    public void ResetPattern()
    {
        SetPattern(pattern);
    }
    public void SetPattern(Pattern patternToPlace)
    {
        if (patternToPlace == null) return;
        pattern = patternToPlace;
        ClearBoard();

        Vector3Int[] placedCells = patternToPlace.GetCellsCentered();

        foreach (var cell in placedCells)
        {
            // place around origin so pattern center -> (0,0)
            currentState.SetTile(cell, aliveTile);
            aliveCells.Add(cell);
        }

        Populations = aliveCells.Count;
        FitCameraToPattern(patternToPlace);
    }
    #endregion

    #region Camera fitting & centering
    private void FitCameraToPattern(Pattern p)
    {
        if (targetCamera == null || p == null) return;
        // We placed pattern centered on origin (0,0). So center is (0,0).
        float aspect = (float)Screen.width / Mathf.Max(1, Screen.height);

        float width = p.GetWidth();
        float height = p.GetHeight();

        // add padding in tiles
        width += cameraPadding;
        height += cameraPadding;

        // orthographicSize is half-vertical size in world units (assuming 1 tile = 1 unit)
        float sizeByHeight = height / 2f;
        float sizeByWidth = (width / 2f) / aspect;

        float neededSize = Mathf.Max(sizeByHeight, sizeByWidth);

        // clamp minimal size to avoid zero
        neededSize = Mathf.Max(neededSize, 1f);

        targetCamera.orthographic = true;
        targetCamera.orthographicSize = neededSize * 4;

        // place camera so that origin is at the center of the view
        Vector3 camPos = new Vector3(0f, 0f, targetCamera.transform.position.z);
        targetCamera.transform.position = camPos;
    }
    #endregion

    #region Simulation loop
    public void StartSimulate()
    {
        simulateCoroutine = StartCoroutine(Simulate());
    }
    public void StopSimulate()
    {
        StopCoroutine(simulateCoroutine);
    }
    private IEnumerator Simulate()
    {
        if (updateInterval <= 0f) yield break;
        yield return new WaitForSeconds(updateInterval);

        while (enabled)
        {
            UpdateNextState();

            yield return new WaitForSeconds(updateInterval);
        }
    }
    public void UpdateNextState()
    {
        UpdateState();
        Populations = aliveCells.Count;
        Iterations++;
        TimeElapsed += updateInterval;
    }
    private void UpdateState()
    {
        // collect cells to check = all alive + their neighbors
        cellsToCheck.Clear();
        foreach (var cell in aliveCells)
        {
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                    cellsToCheck.Add(cell + new Vector3Int(x, y, 0));
        }

        // decide new alive set without mutating aliveCells while iterating
        var newAlive = new HashSet<Vector3Int>();

        foreach (var cell in cellsToCheck)
        {
            int neighbors = CountNeighbors(cell);
            bool alive = aliveCells.Contains(cell);

            if (!alive && neighbors == 3)
            {
                // reproduction
                newAlive.Add(cell);
            }
            else if (alive && (neighbors == 2 || neighbors == 3))
            {
                // stays alive
                newAlive.Add(cell);
            }
            // else it dies / stays dead -> do nothing
        }

        // update tilemap to reflect newAlive
        // faster approach: clear all tiles in region touched (we'll clear whole tilemap for simplicity)
        currentState.ClearAllTiles();
        foreach (var a in newAlive)
        {
            currentState.SetTile(a, aliveTile);
        }

        // swap new set in
        aliveCells = newAlive;
    }
    #endregion

    #region Helpers
    private int CountNeighbors(Vector3Int cell)
    {
        int count = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                var n = cell + new Vector3Int(x, y, 0);
                if (aliveCells.Contains(n)) count++;
            }
        }
        return count;
    }
    #endregion
}
