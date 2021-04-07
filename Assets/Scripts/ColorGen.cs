using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorGen
{
    public static Color[] GenerateColorMapFromHeightMap(float[,] heightMap, TerrainType[] regions)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentHeight = heightMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * width + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        return colorMap;
    }

    public static Color[] GenerateColorMapFromVertices(Vector3[] vertices, TerrainType[] regions,
        float heightMultiplier, AnimationCurve heightCurve)
    {
        Color[] colorMap = new Color[vertices.Length];
        for (int v = 0; v < vertices.Length; v++)
        {
            float currentHeight = vertices[v].y / heightMultiplier;
            for (int i = 0; i < regions.Length; i++)
            {
                if (currentHeight <= heightCurve.Evaluate(regions[i].height))
                {
                    colorMap[v] = regions[i].colour;
                    break;
                }
            }
        }

        return colorMap;
    }
}
