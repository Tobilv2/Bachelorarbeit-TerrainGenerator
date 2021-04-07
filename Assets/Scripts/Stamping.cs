using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Random = UnityEngine.Random;


[ExecuteInEditMode]
public class Stamping : MonoBehaviour
{
    public enum StampMode
    {
        Nothing,
        Flatten,
        Mountain,
        Paint
    };

    public StampMode stampMode;
    public int radius;

    [HideInInspector] [Range(0, 20f)] public float persistance;
    [HideInInspector] [Range(0.001f, 1f)] public float heightMultiplier;
    [HideInInspector] public float flattenStepSize;
    [HideInInspector] public Color paintColor;

    private TerrainGenerator terrainGenerator;

    private void Start()
    {
        terrainGenerator = gameObject.GetComponent<TerrainGenerator>();
    }

    private void OnEnable()
    {
        if (!Application.isEditor)
        {
            Destroy(this);
        }

        SceneView.duringSceneGui += OnScene;
    }

    void OnScene(SceneView scene)
    {
        if (stampMode == StampMode.Nothing)
        {
            return;
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Event e = Event.current;
        terrainGenerator = gameObject.GetComponent<TerrainGenerator>();
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector3 mousePos = e.mousePosition;
            float ppp = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = scene.camera.pixelHeight - mousePos.y * ppp;
            mousePos.x *= ppp;

            Ray ray = scene.camera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Mesh mesh = hit.collider.gameObject.GetComponent<MeshFilter>().sharedMesh;
                MeshRenderer meshRenderer = hit.collider.gameObject.GetComponent<MeshRenderer>();
                MeshFields meshFields = hit.collider.gameObject.GetComponent<MeshFields>();
                MeshCollider meshCollider = hit.collider.gameObject.GetComponent<MeshCollider>();
                Transform trans = hit.collider.gameObject.transform;
                Vector3[] verts = mesh.vertices;
                List<VertexData> verticesToChange = new List<VertexData>();
                float nearestDistance = Mathf.Infinity;
                for (int i = 0; i < verts.Length; i++)
                {
                    Vector3 worldPt = trans.TransformPoint(verts[i]);
                    float dist = Vector2.Distance(new Vector2(worldPt.x, worldPt.z),
                        new Vector2(hit.point.x, hit.point.z));
                    if (dist < radius)
                    {
                        if (stampMode == StampMode.Mountain)
                        {
                            verts[i].y += (radius - dist) * heightMultiplier + Random.Range(0, persistance);
                            meshFields.MeshData.colorMap[i] = checkColorFromHeight(verts[i].y, terrainGenerator.regions,
                                terrainGenerator.meshHeightCurve, terrainGenerator.meshHeightMultiplier);
                        }

                        if (stampMode == StampMode.Flatten)
                        {
                            bool nearestToCursor = false;
                            if (dist < nearestDistance)
                            {
                                nearestDistance = dist;
                                nearestToCursor = true;
                            }

                            VertexData vertex = new VertexData(i, verts[i], nearestToCursor);
                            verticesToChange.Add(vertex);
                        }

                        if (stampMode == StampMode.Paint)
                        {
                            meshFields.MeshData.colorMap[i] = paintColor;
                        }
                    }
                }

                if (stampMode == StampMode.Flatten)
                {
                    float referenceY = 0f;
                    foreach (VertexData vertexData in verticesToChange.Where(vertexData => vertexData.closestToCursor))
                    {
                        referenceY = vertexData.position.y;
                    }

                    foreach (VertexData vertexData in verticesToChange)
                    {
                        float dif = referenceY - verts[vertexData.index].y;
                        if (Math.Abs(dif) < flattenStepSize)
                        {
                            verts[vertexData.index].y = referenceY;
                        }
                        else if (dif < 0)
                        {
                            verts[vertexData.index].y -= flattenStepSize;
                        }
                        else if (dif > 0)
                        {
                            verts[vertexData.index].y += flattenStepSize;
                        }

                        meshFields.MeshData.colorMap[vertexData.index] = checkColorFromHeight(verts[vertexData.index].y,
                            terrainGenerator.regions,
                            terrainGenerator.meshHeightCurve, terrainGenerator.meshHeightMultiplier);
                    }
                }

                mesh.vertices = verts;
                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                meshCollider.sharedMesh = mesh;
                Texture2D meshTexture = TextureGen.TextureFromColorMap(meshFields.MeshData.colorMap,
                    meshFields.MeshData.dims.x, (int) mesh.bounds.size.z);
                meshRenderer.sharedMaterial.mainTexture = meshTexture;
            }

            e.Use();
        }

        HandleUtility.Repaint();
    }

    private static Color checkColorFromHeight(float height, TerrainType[] regions, AnimationCurve heightCurve,
        float heightMulti)
    {
        height /= heightMulti;
        for (int i = 0; i < regions.Length; i++)
        {
            if (height <= heightCurve.Evaluate(regions[i].height))
            {
                return regions[i].colour;
            }
        }

        return regions[regions.Length - 1].colour;
    }

    struct VertexData
    {
        public VertexData(int index, Vector3 position, bool closestToCursor)
        {
            this.index = index;
            this.position = position;
            this.closestToCursor = closestToCursor;
        }

        public readonly int index;
        public readonly Vector3 position;
        public readonly bool closestToCursor;
    }
}