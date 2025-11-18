using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public static class PatternImporter
{
    [MenuItem("Tools/Import RLE Pattern")]
    public static void ImportRLEPattern()
    {
        string path = EditorUtility.OpenFilePanel("Select RLE Pattern File", "", "rle");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path);
        List<Vector2Int> cells = new List<Vector2Int>();

        int y = 0;
        int x = 0;
        foreach (string line in lines)
        {
            if (line.StartsWith("#") || line.StartsWith("x")) continue; // skip comments and header

            int count = 0;
            foreach (char c in line)
            {
                if (char.IsDigit(c))
                {
                    count = count * 10 + (c - '0');
                }
                else
                {
                    if (count == 0) count = 1;

                    if (c == 'b')
                        x += count; // dead cells, just skip
                    else if (c == 'o')
                    {
                        for (int i = 0; i < count; i++)
                            cells.Add(new Vector2Int(x++, -y));
                    }
                    else if (c == '$')
                    {
                        y += count;
                        x = 0;
                    }
                    else if (c == '!')
                        break;

                    count = 0;
                }
            }
        }

        Pattern pattern = ScriptableObject.CreateInstance<Pattern>();
        pattern.cells = cells.ToArray();

        string assetPath = $"Assets/Patterns/{Path.GetFileNameWithoutExtension(path)}.asset";
        Directory.CreateDirectory("Assets/Patterns");
        AssetDatabase.CreateAsset(pattern, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"Imported pattern: {assetPath} ({cells.Count} cells)");
    }
}
