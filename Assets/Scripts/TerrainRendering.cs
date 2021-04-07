using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Linq;


public class TerrainRendering : MonoBehaviour
{
   
    public GameObject terrainParent;
    public GameObject meshPrefab;

    



    public void DrawMesh(List<MeshData> meshDatas, float meshScale, Material shaderMaterial)
    {
        var tempList = terrainParent.transform.Cast<Transform>().ToList();
        foreach (Transform child in tempList) {
            GameObject.DestroyImmediate(child.gameObject);
        }
        terrainParent.transform.localScale = new Vector3(1,1,1);

        foreach (MeshData data in meshDatas)
        {
            Vector3 offset = Vector3.zero;
            if (data.chunkCoords.x > 0)
            {
                offset.x = NoiseGen.MAX_CHUNK_SIZE * (data.chunkCoords.x - 1) + NoiseGen.MAX_CHUNK_SIZE/2f + 0.5f * data.dims.x;
                offset.x -= 0.5f;
            }

            if (data.chunkCoords.y > 0)
            {
                offset.z = -(NoiseGen.MAX_CHUNK_SIZE * (data.chunkCoords.y - 1) + NoiseGen.MAX_CHUNK_SIZE/2f + 0.5f * data.dims.y);
                offset.z += 0.5f;
            }
            GameObject mesh = Instantiate(meshPrefab, offset, Quaternion.identity);
            mesh.transform.parent = terrainParent.transform;
            MeshFilter meshFilter = mesh.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = mesh.GetComponent<MeshRenderer>();
            MeshCollider meshCollider = mesh.GetComponent<MeshCollider>();
            MeshFields meshFields = mesh.GetComponent<MeshFields>();
            meshFields.MeshData = data;
            Mesh meshObject = data.CreateMesh();
            meshFilter.sharedMesh = meshObject;
            meshCollider.sharedMesh = meshObject;
            if (shaderMaterial != null)
            {
                meshRenderer.material = shaderMaterial;
            }
            else
            {
                meshRenderer.material = new Material(Shader.Find("Standard"));
                meshRenderer.sharedMaterial.SetFloat("_Glossiness", 0f);
                meshRenderer.sharedMaterial.mainTexture = data.texture;
            }
        }
        terrainParent.transform.localScale = new Vector3(meshScale,1,meshScale);
        
    }
    
}
