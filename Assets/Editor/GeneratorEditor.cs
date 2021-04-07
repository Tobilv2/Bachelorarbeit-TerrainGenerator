using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
// ReSharper disable All

[CustomEditor(typeof(TerrainGenerator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator) target;

        if (DrawDefaultInspector())
        {
            if (terrainGenerator.autoUpdate)
            {
                terrainGenerator.GenerateTerrain();
            }
        }

        if (GUILayout.Button("Generate Terrain"))
        {
            terrainGenerator.GenerateTerrain();
        }
        if (GUILayout.Button("Generate Foliage"))
        {
            terrainGenerator.GenerateFoliage();
        }
    }


}
