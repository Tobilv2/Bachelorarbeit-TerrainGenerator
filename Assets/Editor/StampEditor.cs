using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Stamping))]
public class StampEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Stamping stamping = (Stamping) target;
        if (stamping.stampMode == Stamping.StampMode.Flatten)
        {
            stamping.flattenStepSize = EditorGUILayout.FloatField("Flatting Step Size: ", stamping.flattenStepSize);
        }
        if (stamping.stampMode == Stamping.StampMode.Mountain)
        {
            stamping.heightMultiplier = EditorGUILayout.Slider("Height Multiplier: ", stamping.heightMultiplier,0.001f,1f);
            stamping.persistance = EditorGUILayout.Slider("Persistance: ", stamping.persistance,0f,20f);
        }
        if (stamping.stampMode == Stamping.StampMode.Paint)
        {
            stamping.paintColor = EditorGUILayout.ColorField("Color: ", stamping.paintColor);
        }
    }
}
