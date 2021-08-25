using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using static Define;

[CustomEditor(typeof(MapMeshDataWrapper))]
public class MapMeshDataWrapperEditor : Editor
{
    private int _selectedIndex = 0;

    private readonly string[] options = Enum.GetNames(typeof(MapMeshType));

    MapMeshDataWrapper mmdw;

    private void OnEnable()
    {
        mmdw = (MapMeshDataWrapper)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //GUI.Label(position, label);

        DrawProperty();


        if (GUI.changed)
        {
            EditorUtility.SetDirty(mmdw);
        }
    }

    private void DrawProperty()
    {

        switch (mmdw.meshType)
        {
            case MapMeshType.Tunnel:
                {
                    TorusMesh mesh = (TorusMesh)mmdw.GetMesh();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Tunnel Radius", GUILayout.MaxWidth(90));
                    mesh.mapSize = EditorGUILayout.FloatField(mesh.mapSize, GUILayout.MaxWidth(30));
                    EditorGUILayout.LabelField("Tunnel Segment Count", GUILayout.MaxWidth(140));
                    mesh.roadSegmentCount = EditorGUILayout.IntField(mesh.roadSegmentCount, GUILayout.MaxWidth(30));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Ring Distance", GUILayout.MaxWidth(90));
                    mesh.ringDistance = EditorGUILayout.FloatField(mesh.ringDistance, GUILayout.MaxWidth(30));
                    EditorGUILayout.EndHorizontal();


                    GUIStyle style = new GUIStyle();
                    style.fontStyle = FontStyle.Bold;
                    style.normal.textColor = Color.white;

                    EditorGUILayout.LabelField("Curve Radius", style, GUILayout.MaxWidth(100));
                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUILayout.LabelField("min", GUILayout.MaxWidth(30));
                    mesh.minCurveRadius = EditorGUILayout.FloatField(mesh.minCurveRadius, GUILayout.MaxWidth(30));
                    EditorGUILayout.LabelField("max", GUILayout.MaxWidth(30));
                    mesh.maxCurveRadius = EditorGUILayout.FloatField(mesh.maxCurveRadius, GUILayout.MaxWidth(30));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField("Curve Segment Count", style, GUILayout.MaxWidth(150));
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("min", GUILayout.MaxWidth(30));
                    mesh.minCurveSegmentCount = EditorGUILayout.IntField(mesh.minCurveSegmentCount, GUILayout.MaxWidth(30));
                    EditorGUILayout.LabelField("max", GUILayout.MaxWidth(30));
                    mesh.maxCurveSegmentCount = EditorGUILayout.IntField(mesh.maxCurveSegmentCount, GUILayout.MaxWidth(30));
                    EditorGUILayout.EndHorizontal();


                    break;
                }
            case MapMeshType.Plane:
                {
                    break;
                }
            case MapMeshType.ReverseTunnel:
                {
                    break;
                }
        }

    }

}
