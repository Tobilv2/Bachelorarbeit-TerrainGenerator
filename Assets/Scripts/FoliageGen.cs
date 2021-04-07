using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;


public class FoliageGen : MonoBehaviour
{
    public static GameObject treeParent;
    private const float GENERAL_DENSITY = 0.15f;

    public static void GenerateFoliage(float[,] treeNoiseMap, TerrainType[] regions, float forestAmount, float mapScale, AnimationCurve heightCurve, float heightMulti)
    {
        
        removeAllFoliage();
        int width = treeNoiseMap.GetLength(0);
        int height = treeNoiseMap.GetLength(1);
        
        
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (treeNoiseMap[x, y] > forestAmount)
                {
                    if (Random.Range(0f, 1f) < GENERAL_DENSITY)
                    {
                        RaycastHit hit;
                        Vector3 rayOrigin = new Vector3((x - 125) * mapScale, 1000f,
                            (y - width+125) * mapScale);
                        if (Physics.Raycast(rayOrigin, Vector3.down, out hit,
                            1100f))
                        {
                            GameObject prefabToInstantiate =
                                checkIfTreeFromHeight(hit.point.y, regions, heightCurve, heightMulti);
                            if (prefabToInstantiate != null)
                            {
                                GameObject tree = Instantiate(prefabToInstantiate, hit.point,
                                    Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
                                tree.transform.localScale *= mapScale / 2;
                                tree.transform.parent = treeParent.transform;
                            }
                        }
                    }
                }
            }
        }
    }
    
    private static GameObject checkIfTreeFromHeight(float height, TerrainType[] regions, AnimationCurve heightCurve, float heightMulti)
    {
        height /= heightMulti;
        for (int i = 0; i < regions.Length; i++)
        {
            if (height <= heightCurve.Evaluate(regions[i].height))
            {
                if (Random.Range(0, 1f) < regions[i].foliageDensity && regions[i].foliagePrefab != null)
                {
                    return regions[i].foliagePrefab;
                }
                else
                {
                    return null;
                }
            }
        }

        if(Random.Range(0f,1f) < regions[regions.Length-1].foliageDensity)
        {
            return regions[regions.Length - 1].foliagePrefab;
        }

        return null;
    }

    public static void removeAllFoliage()
    {
        if (treeParent == null)
        {
            treeParent = GameObject.Find("TreeParent");
            
        }
        var tempList = treeParent.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList) {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
}
