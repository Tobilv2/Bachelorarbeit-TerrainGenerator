using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGen
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMulti, AnimationCurve heightCurve, Vector2 chunkCoords, TerrainType[] regions)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;
        Texture2D meshTexture = TextureGen.TextureFromColorMap(ColorGen.GenerateColorMapFromHeightMap(heightMap, regions), width, height);
        Color[] colorMap = ColorGen.GenerateColorMapFromHeightMap(heightMap, regions);
        MeshData currentMeshData = new MeshData(width, height, chunkCoords, meshTexture, colorMap);
        int vertexIndex = 0;
        for (int y  = 0; y  < height; y ++)
        {
            for (int x  = 0; x  < width; x ++)
            {
                currentMeshData.vertices[vertexIndex] = new Vector3(topLeftX+x,
                    heightCurve.Evaluate(heightMap[x,y])*heightMulti,topLeftZ-y);
                currentMeshData.uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);
                
                if (x < width - 1 && y < height - 1)
                {
                    currentMeshData.AddTriangle(vertexIndex, vertexIndex+width+1, vertexIndex+width);
                    currentMeshData.AddTriangle(vertexIndex+width+1,vertexIndex,vertexIndex+1);
                }

                vertexIndex++;
            }
        }
        
        return currentMeshData;
    }
    
}

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;
    public Vector2Int dims;
    public Vector2 chunkCoords;
    public Color[] colorMap;
    public Texture2D texture;
    private int triangleIndex;
    
    public MeshData(int meshWidth, int meshHeight, Vector2 chunkCoords, Texture2D texture, Color[] colorMap)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth*meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        dims = new Vector2Int(meshWidth, meshHeight);
        this.chunkCoords = chunkCoords;
        this.texture = texture;
        this.colorMap = colorMap;

    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
