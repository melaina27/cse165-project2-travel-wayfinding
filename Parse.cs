using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Parse : MonoBehaviour
{
    public string filePath; // full path to .xyz file

    public List<Vector3> ParseFile()
    {
        float ScaleFactor = 1.0f / 39.37f;
        List<Vector3> positions = new List<Vector3>();

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return positions;
        }

        string content = File.ReadAllText(filePath);
        string[] lines = content.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] coords = line.Split(
                new char[] { ' ', '\t' },
                System.StringSplitOptions.RemoveEmptyEntries
            );

            if (coords.Length < 3) continue;

            Vector3 pos = new Vector3(
                float.Parse(coords[0]),
                float.Parse(coords[1]),
                float.Parse(coords[2])
            );

            positions.Add(pos * ScaleFactor);
        }

        Debug.Log($"Loaded {positions.Count} checkpoints from {filePath}");
        return positions;
    }
}
