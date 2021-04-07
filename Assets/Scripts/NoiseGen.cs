using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable All

public static class NoiseGen
{
    public const int MAX_CHUNK_SIZE = 250;

    public static float[,] GenerateNoiseMap(int width, int height, int seed, float scale, int octaves,
        float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = width / 2;
        float halfHeight = height / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float mapX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float mapY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float value = Mathf.PerlinNoise(mapX, mapY);
                    noiseHeight += value * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    public static List<ChunkData> ChunkNoiseMap(float[,] noiseMap)
    {
        List<ChunkData> chunkDatas = new List<ChunkData>();

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        int numberOfMaxX = (width / MAX_CHUNK_SIZE);

        if (width % MAX_CHUNK_SIZE > 0)
        {
            numberOfMaxX++;
        }

        int numberOfMaxY = (height / MAX_CHUNK_SIZE);
        if (height % MAX_CHUNK_SIZE > 0)
        {
            numberOfMaxY++;
        }

        for (int y = 1; y <= numberOfMaxY; y++)
        {
            for (int x = 1; x <= numberOfMaxX; x++)
            {
                int tempWidth;
                int tempHeight;
                if (numberOfMaxX == 1)
                {
                    tempWidth = width;
                }
                else if (x == numberOfMaxX && width % MAX_CHUNK_SIZE > 0)
                {
                    tempWidth = width % MAX_CHUNK_SIZE;
                }
                else
                {
                    tempWidth = MAX_CHUNK_SIZE;
                }

                if (numberOfMaxY == 1)
                {
                    tempHeight = height;
                }
                else if (y == numberOfMaxY && height % MAX_CHUNK_SIZE > 0)
                {
                    tempHeight = height % MAX_CHUNK_SIZE;
                }
                else
                {
                    tempHeight = MAX_CHUNK_SIZE;
                }

                if (x != numberOfMaxX)
                {
                    tempWidth++;
                }

                if (y != numberOfMaxY)
                {
                    tempHeight++;
                }

                float[,] chunkedNoiseMap = new float[tempWidth, tempHeight];
                for (int i = 0; i < chunkedNoiseMap.GetLength(0); i++)
                {
                    for (int j = 0; j < chunkedNoiseMap.GetLength(1); j++)
                    {
                        chunkedNoiseMap[i, j] = noiseMap[(x - 1) * MAX_CHUNK_SIZE + i, (y - 1) * MAX_CHUNK_SIZE + j];
                    }
                }
                ChunkData chunk = new ChunkData(chunkedNoiseMap, new Vector2(x - 1, y - 1));
                chunkDatas.Add(chunk);
            }
        }

        return chunkDatas;
    }

    public static float[,] GenerateTreeNoiseMap(int width, int height)
    {
        float[,] noiseMap = new float[width, height];

        float scale = 50f;
        System.Random prng = new System.Random();
        float offset = prng.Next(-100000, 100000);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float sampleX = x / scale + offset;
                float sampleY = y / scale + offset;
                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }

        return noiseMap;
    }
}


public struct ChunkData
{
    public Vector2 chunkCoords;
    public float[,] heightMap;

    public ChunkData(float[,] heightMap, Vector2 chunkCoords)
    {
        this.chunkCoords = chunkCoords;
        this.heightMap = heightMap;
    }
}