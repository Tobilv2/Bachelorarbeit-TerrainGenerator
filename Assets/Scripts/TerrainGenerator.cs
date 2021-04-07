using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("General Settings: ")]

    public int width;
    public int height;
    public int seed;
    public Vector2 offset;
    public float meshScale;
    public float forestAmount;
    
    [Header("Noise Settings: ")]
    public float scale;
    public int octaves;
    [Range(0,1)]
    public float persistance;
    public float lacunarity;
    
    [Header("Mesh Settings: ")]
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public TerrainType[] regions;
    public bool useShader;
    public Material shaderMaterial;
    
    public bool autoUpdate;
    public void GenerateTerrain()
    {
        float[,] noiseMap =
            NoiseGen.GenerateNoiseMap(width, height, seed, scale, octaves, persistance, lacunarity, offset);

        FoliageGen.removeAllFoliage();
        
        TerrainRendering terrainRendering = FindObjectOfType<TerrainRendering>();


            List<ChunkData> chunks = NoiseGen.ChunkNoiseMap(noiseMap);
            List<MeshData> meshData = new List<MeshData>();
            foreach (ChunkData chunkData in chunks)
            {
                meshData.Add(MeshGen.GenerateTerrainMesh(chunkData.heightMap, meshHeightMultiplier, meshHeightCurve, chunkData.chunkCoords,regions));
            }

            if (useShader)
            {
                terrainRendering.DrawMesh(meshData, meshScale, shaderMaterial);
            }
            else
            {
                terrainRendering.DrawMesh(meshData, meshScale, null);
            }


        UpdateShaderData();
    }

    public void GenerateFoliage()
    {
        float[,] treeNoiseMap = NoiseGen.GenerateTreeNoiseMap(width, height);
        FoliageGen.GenerateFoliage(treeNoiseMap,regions,forestAmount,meshScale, meshHeightCurve, meshHeightMultiplier);

    }

    public void UpdateShaderData()
    {
        shaderMaterial.SetFloat("minHeight", meshHeightMultiplier*meshHeightCurve.Evaluate(0));
        shaderMaterial.SetFloat("maxHeight",meshHeightMultiplier*meshHeightCurve.Evaluate(1));
        float[] startHeights = new float[regions.Length];
        Color[] colors = new Color[regions.Length];
        float[] blends = new float[regions.Length];
        for (int i = 0; i < regions.Length; i++)
        {
            if (i == 0)
            {
                startHeights[i] = 0f;
                
            }
            else
            {
                startHeights[i] = meshHeightCurve.Evaluate(regions[i - 1].height);
            }

            colors[i] = regions[i].colour;
            blends[i] = regions[i].blend;
        }
        
        shaderMaterial.SetFloatArray("baseStartHeights", startHeights);
        shaderMaterial.SetFloatArray("baseBlends", blends);
        shaderMaterial.SetColorArray("baseRegions", colors);
        shaderMaterial.SetFloat("baseRegionCount", regions.Length);
    }

    private void OnValidate()
    {
        if (width < 1)
        {
            width = 1;
        }
        if (height < 1)
        {
            height = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }

        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
    [Range(0f,1f)] public float blend; 
    [Range(0f,1f)] public float foliageDensity;
    public GameObject foliagePrefab;
}
