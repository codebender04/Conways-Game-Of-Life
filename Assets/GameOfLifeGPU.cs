using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOfLifeGPU : MonoBehaviour
{
    [Header("Compute")]
    public ComputeShader computeShader;
    public string kernelName = "CSMain";

    [Header("Grid")]
    public int width = 1024;
    public int height = 1024;

    [Header("Rendering")]
    public Material displayMaterial; // material used on Quad or RawImage that will show the result
    public RawImage rawImageTarget; // optional: assign if using UI
    public bool useLinearFiltering = false;

    [Header("Simulation")]
    [Range(0, 100)]
    public int stepsPerFrame = 1; // how many Life steps to simulate each frame
    public bool autoSimulate = true;

    [Header("Init")]
    public Pattern initialPattern;    // your ScriptableObject pattern (optional)
    public Vector2Int patternOffset = new Vector2Int(512, 512); // center where pattern will be placed
    public Color aliveColor = Color.white;
    public Color deadColor = Color.black;

    private RenderTexture rtA;
    private RenderTexture rtB;
    private int kernel;
    private bool ping = true;

    void Start()
    {
        if (computeShader == null)
        {
            Debug.LogError("Assign compute shader.");
            enabled = false;
            return;
        }

        kernel = computeShader.FindKernel(kernelName);
        CreateRenderTextures();
        Clear(rtA);
        Clear(rtB);

        if (initialPattern != null)
            LoadPatternToTexture(initialPattern, rtA, patternOffset);

        // show initial texture on material / raw image
        ApplyDisplayTexture(rtA);
    }

    void CreateRenderTextures()
    {
        ReleaseRT(ref rtA);
        ReleaseRT(ref rtB);

        rtA = CreateRTPointer(width, height);
        rtB = CreateRTPointer(width, height);
    }

    RenderTexture CreateRTPointer(int w, int h)
    {
        RenderTexture rt = new RenderTexture(w, h, 0, RenderTextureFormat.ARGB32);
        rt.enableRandomWrite = true;
        rt.filterMode = useLinearFiltering ? FilterMode.Bilinear : FilterMode.Point;
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.Create();
        return rt;
    }

    void ReleaseRT(ref RenderTexture rt)
    {
        if (rt != null)
        {
            rt.Release();
            Destroy(rt);
            rt = null;
        }
    }

    void OnDestroy()
    {
        ReleaseRT(ref rtA);
        ReleaseRT(ref rtB);
    }

    void Update()
    {
        if (computeShader == null || displayMaterial == null) return;

        if (autoSimulate)
        {
            for (int i = 0; i < stepsPerFrame; i++)
                StepGPU();
        }
        else
        {
            // nothing: allow manual StepGPU() invocation
        }

        // update the display to the currently active (ping)
        ApplyDisplayTexture(GetCurrentRT());
    }

    // Run single step on GPU (ping-pong)
    public void StepGPU()
    {
        RenderTexture src = GetCurrentRT();
        RenderTexture dst = GetNextRT();

        computeShader.SetInt("_Width", width);
        computeShader.SetInt("_Height", height);
        computeShader.SetTexture(kernel, "_Source", src);
        computeShader.SetTexture(kernel, "_Dest", dst);

        int threadGroupsX = Mathf.CeilToInt(width / 8f);
        int threadGroupsY = Mathf.CeilToInt(height / 8f);
        computeShader.Dispatch(kernel, threadGroupsX, threadGroupsY, 1);

        ping = !ping;
    }

    RenderTexture GetCurrentRT() => ping ? rtA : rtB;
    RenderTexture GetNextRT() => ping ? rtB : rtA;

    void ApplyDisplayTexture(RenderTexture rt)
    {
        if (displayMaterial != null)
        {
            displayMaterial.mainTexture = rt;
        }
        if (rawImageTarget != null)
        {
            rawImageTarget.texture = rt;
        }
    }

    // Clear a RenderTexture (set dead)
    void Clear(RenderTexture rt)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, deadColor);
        RenderTexture.active = prev;
    }

    // Randomize pattern
    public void Randomize(float aliveProbability = 0.1f)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] cols = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool a = Random.value < aliveProbability;
                cols[y * width + x] = a ? aliveColor : deadColor;
            }
        }
        tex.SetPixels(cols);
        tex.Apply();
        Graphics.Blit(tex, rtA); // initialize rtA
        ping = true;
        Destroy(tex);
    }

    // Load a Pattern ScriptableObject (your Pattern.cells: Vector2Int[]) into the given RenderTexture
    public void LoadPatternToTexture(Pattern pattern, RenderTexture target, Vector2Int offset)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] cols = new Color[width * height];
        // default dead
        for (int i = 0; i < cols.Length; i++) cols[i] = deadColor;

        if (pattern != null && pattern.cells != null)
        {
            Vector2Int center = pattern.GetCenter();
            for (int i = 0; i < pattern.cells.Length; i++)
            {
                Vector2Int p = pattern.cells[i] - center + offset;
                if (p.x >= 0 && p.x < width && p.y >= 0 && p.y < height)
                {
                    cols[p.y * width + p.x] = aliveColor;
                }
            }
        }

        tex.SetPixels(cols);
        tex.Apply();
        Graphics.Blit(tex, target);
        Destroy(tex);
    }

    // Convenience methods for UI or buttons
    public void SetAutoSimulate(bool v) => autoSimulate = v;
    public void SetStepsPerFrame(int s) => stepsPerFrame = Mathf.Max(1, s);
    public void ClearAll() { Clear(rtA); Clear(rtB); ping = true; }
    public void StepOnce() { StepGPU(); ApplyDisplayTexture(GetCurrentRT()); }
}
